/**
 * Created by paul on 5/7/15.
 */
'use strict';

var dashboardModule = require('./index');
var Dashboard = Dashboard;

Dashboard.$inject = ['$scope', '$state', '$http'];

function Dashboard ($scope, $state, $http) {
    var vm = this;

    vm.fileToUpload = undefined;

    function uploadFile() {
        var fd = new FormData();
        fd.append('file', vm.fileToUpload);
        $http.post('http://localhost:55328/api/file', fd, {
            transformRequest: angular.identity,
            headers: { 'Content-Type': undefined }
        })
        .success(function () {
        })
        .error(function () {
        });
    }
    
    vm.upload = uploadFile;
    vm.cancel = function () {
        console.log(vm.fileToUpload);
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