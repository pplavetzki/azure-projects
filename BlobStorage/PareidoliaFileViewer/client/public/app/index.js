'use strict';

var angular = require('angular');

require('./modules/core');
require('./modules/layout');
require('./modules/dashboard');

function setup() { }

angular
        .module('app', ['app.core',
                        'app.layout',
                        'app.dashboard'
        ])
        .run(setup);