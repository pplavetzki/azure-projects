/**
 * Created by paul on 5/7/15.
 */
'use strict';

var dashboardModule = require('./index');
var Dashboard = Dashboard;

Dashboard.$inject = ['$window', '$scope', '$state', '$http', 'AzureBlob', 'constants'];

function Dashboard ($window, $scope, $state, $http, azureBlob, constants) {
    var vm = this;

    vm.fileToUpload = undefined;
    vm.files = undefined;

    function activate() {
        $http.get(constants.apiBaseUrl + 'file')
            .success(function (results) {
                vm.files = results;
            })
            .error(function () {

            });
    }

    function uploadFile() {
        vm.files.push({fileName:vm.fileToUpload.name});
        $http.get(constants.apiBaseUrl + 'file/SASToken?fileName=' + $window.encodeURIComponent(vm.fileToUpload.name))
        .success(function (results) {
            console.log(results);
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
                        console.log("Completed!");
                        var image = {
                            id: results.id,
                            blobUrl: results.blobUrl,
                            fileName: vm.fileToUpload.name,
                            thumbnailUrl:null
                        };
                        $http.post(constants.apiBaseUrl + 'file', image)
                        .success(function (data) {
                            console.log(data);
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