/**
 * Created by paul on 5/7/15.
 */
'use strict';

var angular = require('angular');

module.exports = angular.module('app.layout', ['app.core']);

require('./layout.routes');
require('./shell.controller');