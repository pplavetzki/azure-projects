'use strict';

var coreModule = require('./index');
var Constants = constants;

function constants() {

    var appBaseUrl = 'http://localhost:55328/';
    var apiBaseUrl = appBaseUrl + "api/";

    return {
        appBaseUrl: appBaseUrl,
        apiBaseUrl: apiBaseUrl
    };

}

coreModule.constant('constants', Constants());