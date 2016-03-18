'use strict';

var angular = require('angular');
require('angular-animate');


require('./modules/core');
require('./modules/layout');
require('./modules/dashboard');
require('./modules/directives');
require('./modules/widgets');

function setup() { }

angular
        .module('app', ['app.core',
                        'app.layout',
                        'app.dashboard',
                        'app.directives',
                        'app.widgets',
                        'ngAnimate'
        ])
        .run(setup);