'use strict';

var coreModule = require('./index');
var Constants = constants;

function constants() {

    return {
        appBaseUrl: 'http://localhost:52286/',
        apiBaseUrl: 'http://localhost:52286/api/'
    };

}

coreModule.constant('constants', Constants());