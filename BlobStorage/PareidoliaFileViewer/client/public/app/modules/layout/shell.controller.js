/**
 * Created by paul on 5/7/15.
 */
'use strict';

var layoutModule = require('./index');

var Shell = Shell;

Shell.$inject = ['$rootScope', '$timeout', '$window', '$state'];

function Shell($rootScope, $timeout, $window, $state) {
    var vm = this;

    vm.logout = logout;
    vm.click = register;
    
    function register() {
        $state.go('registration');
    }

    function activate(){
        $timeout(function(){
            vm.show = true;
        }, 400);
    }

    function logout(){
        $window.sessionStorage.token = null;
        $state.go('login')
    };

    activate();

};

layoutModule.controller('Shell', Shell);