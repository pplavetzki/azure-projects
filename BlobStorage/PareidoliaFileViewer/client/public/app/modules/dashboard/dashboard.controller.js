/**
 * Created by paul on 5/7/15.
 */
'use strict';

var dashboardModule = require('./index');
var Dashboard = Dashboard;

Dashboard.$inject = ['$window', '$scope', '$state', '$http', '$interval', 'AzureBlob', 'constants'];

function Dashboard ($window, $scope, $state, $http, $interval, azureBlob, constants) {
    var vm = this;

    var thumbnailIntervals = [];
    var intervalCount = 0;

    vm.fileToUpload = undefined;
    vm.files = undefined;
    vm.bool = false;

    function destroy() {
        console.log("Destroyed!");
    }

    $scope.$on('$destroy', destroy);

    function getThumbnailUrl(imageId, count) {
        $http.get(constants.apiBaseUrl + 'file/GetThumbnailUrl/' + imageId)
            .success(function (thumbnailUrl) {
                console.log(thumbnailUrl);
                if (thumbnailUrl) {
                    stopInterval(count);
                    vm.files[(vm.files.length - 1)].thumbnailUrl = thumbnailUrl;
                }
            })
            .error(function () {

            });
    }

    function stopInterval(index) {
        $interval.cancel(thumbnailIntervals[index]);
        thumbnailIntervals.pop();
    }

    function activate() {
        $http.get(constants.apiBaseUrl + 'file')
            .success(function (results) {
                vm.files = results;
                vm.bool = true;
            })
            .error(function () {

            });
    }

    function uploadFile() {
        $http.get(constants.apiBaseUrl + 'file/SASToken?fileName=' + $window.encodeURIComponent(vm.fileToUpload.name))
        .success(function (results) {
            vm.files.push({ thumbnailUrl: '', blobUrl: results.blobUrl, fileName: vm.fileToUpload.name });
            vm.config =
                {
                    baseUrl: results.blobUrl,
                    sasToken: results.sasToken,
                    file: vm.fileToUpload,
                    blockSize: 1024 * 32,

                    progress: function (amount) {
                        console.log("Progress - " + amount);
                        vm.progress = amount;
                        console.log(amount);
                    },
                    complete: function () {
                        var image = {
                            id: results.id,
                            blobUrl: results.blobUrl,
                            fileName: vm.fileToUpload.name,
                            blobName: results.fileName,
                            thumbnailUrl:null
                        };
                        $http.post(constants.apiBaseUrl + 'file', image)
                        .success(function (data) {
                            thumbnailIntervals.push($interval(getThumbnailUrl, 1000, 10, null, results.id, intervalCount++));
                        })
                        .error(function () {
                        });
                    },
                    error: function (data, status, err, config) {
                        console.log("Error - " + data);
                        vm.error = data;
                    }
                };
            azureBlob.upload(vm.config);
        })
        .error(function () {

        });
    }
    
    vm.upload = uploadFile;
    vm.cancel = function () {
        console.log($window.encodeURIComponent(vm.fileToUpload.name));
    };

    activate();

    vm.gotoAnalysis = function() {
        $state.go('app.workflow');
    };
};

dashboardModule.controller('Dashboard', Dashboard);