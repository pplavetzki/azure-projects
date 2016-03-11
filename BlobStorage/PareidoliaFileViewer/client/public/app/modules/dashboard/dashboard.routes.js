/**
 * Created by paul on 5/7/15.
 */
'use strict';

var dashboardModule = require('./index');

function getStates() {

    return [{
        state: 'app.dashboard',
        config: {
            url: '/',
            title: 'Dashboard',
            views:{
                "content@app": {
                    templateUrl: 'client/build/app/views/modules/dashboard/dashboard.html',
                    controller: 'Dashboard',
                    controllerAs: 'vm'
                }
            }
        }
        }, {
            state: 'auth.greetings',
            config: {
                url: '/greetings',
                title: 'Greetings',
                views: {
                    "content@auth": {
                        templateUrl: 'client/build/app/views/modules/dashboard/greetings.html',
                        controller: 'Greetings',
                        controllerAs: 'vm'
                    }
                }
            }
        }];
}

onRun.$inject = ['routeHelper'];
function onRun(routeHelper){
    routeHelper.configureStates(getStates());
};

dashboardModule
    .run(onRun);