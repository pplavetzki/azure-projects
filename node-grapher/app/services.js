var https = require('https');

module.exports = function() {
    var apiVersion = '';
    var result = '';
    
    function getGroups(token, callback) {
        
        var options = {
            hostname: 'graph.windows.net',
            port: 443,
            path: '//groups?api-version=1.6',
            method: 'GET',
            headers: {
                Authorization: 'Bearer ' + token
            }
        };

        var req = https.request(options, function(res) {
            console.log('statusCode: ', res.statusCode);
            res.setEncoding('utf8');
        
            res.on('data', function(d) {
                    result += d;
                });
                
            res.on('end', function() {
                callback(result);
            });
        });
        
        req.end();
        
        req.on('error', function(e) {
            console.error(e);
        });
    }
    
    return {
        getGroups : getGroups
    }
}
