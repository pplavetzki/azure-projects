/**
 * Created by paul on 5/7/15.
 */
'use strict';

var dashboardModule = require('./index');
var Dashboard = Dashboard;

Dashboard.$inject = ['$scope', '$state', '$http'];

function Dashboard ($scope, $state, $http) {
    var vm = this;

    function uploadFile() {
        var f = document.getElementById('file').files[0],
            r = new FileReader();
            r.onloadend = function (e) {
                var fd = new FormData();
                console.log(e.target.result);
                fd.append('file', e.target.result);
                //var data = e.target.result;
                //$http({
                //    method: 'POST',
                //    headers: { 'Content-Type': undefined },
                //    data: new Uint8Array(data),
                //    transformRequest: [],
                //    url: 'http://localhost:55328/api/file'
                //})
                //send you binary data via $http or $resource or do anything else with it
                $http.post("http://localhost:55328/api/file", fd, {
                    transformRequest: angular.identity,
                    headers: { 'Content-Type': undefined }
                })
                .success(function () {
                    console.log("posted");
                })
                .error(function () {
                    console.log("error");
                });
            }
            r.readAsDataURL(f);
    }
    
    vm.upload = uploadFile;
    vm.cancel = function (e) {
        console.log(e);
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