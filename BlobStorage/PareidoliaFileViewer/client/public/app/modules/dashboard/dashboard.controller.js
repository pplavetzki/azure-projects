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

    function uploadFile() {
        //var fd = new FormData();
        //fd.append('file', vm.fileToUpload);
        //$http.post(constants.apiBaseUrl + 'file', fd, {
        //    transformRequest: angular.identity,
        //    headers: { 'Content-Type': undefined }
        //})
        //.success(function () {
        //})
        //.error(function () {
        //});
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
    /*  
    commonDataService.getValues().then(function (data) {
        vm.data = data;
    });
    

    commonDataService.getValue(1).then(function (data) {
        vm.datum = data;
    });
    */

    vm.gotoAnalysis = function() {
        $state.go('app.workflow');
    };
};

dashboardModule.controller('Dashboard', Dashboard);