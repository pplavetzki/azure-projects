'use strict';

var angular = require('angular');

require('./modules/core');
require('./modules/layout');
require('./modules/dashboard');
require('./modules/directives');

function setup() { }

angular
        .module('app', ['app.core',
                        'app.layout',
                        'app.dashboard',
                        'app.directives'
        ])
        .run(setup);