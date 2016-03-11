'use strict';

var dashboardModule = require('./index');
var Greetings = Greetings;

Greetings.$inject = ['$state'];

function Greetings($state) {
    var vm = this;
   
};

dashboardModule.controller('Greetings', Greetings);