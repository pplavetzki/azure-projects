/**
 * Created by paul on 5/7/15.
 */
'use strict';

var dashboardModule = require('./index');
var Dashboard = Dashboard;

Dashboard.$inject = ['$state', 'commonDataService'];

function Dashboard ($state, commonDataService) {
    var vm = this;
    
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