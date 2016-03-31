
var express = require('express');
var app = express();

var adal = require('adal-node');
var azure = require('azure');

var services = require('./app/services.js')();

var AuthenticationContext = adal.AuthenticationContext;

var port = process.env.PORT || 3000;

var parametersFile = process.argv[2] || process.env['ADAL_SAMPLE_PARAMETERS_FILE'];

var sampleParameters;
if (parametersFile) {
  var jsonFile = fs.readFileSync(parametersFile);
  if (jsonFile) {
    sampleParameters = JSON.parse(jsonFile);
  } else {
    console.log('File not found, falling back to defaults: ' + parametersFile);
  }
}

if (!parametersFile) {
  sampleParameters = {

  };
}

var authorityUrl = sampleParameters.authorityHostUrl + '/' + sampleParameters.tenant;

//var resource = 'https://management.core.windows.net/';
var resource = '00000002-0000-0000-c000-000000000000';

var context = new AuthenticationContext(authorityUrl);

var credentials = undefined;

function listCallback(error, result){
  if (error)
    return console.error('List Error: ', error);

  console.log("List Success: " + JSON.stringify(result));
}

app.get('/', function (req, res) {
  context.acquireTokenWithClientCredentials(resource, sampleParameters.clientId, sampleParameters.clientSecret, function(err, tokenResponse) {
    if (err) {
        console.log('well that didn\'t work: ' + err.stack);
    } else {
        /*
        credentials = new azure.TokenCloudCredentials({
            subscriptionId: sampleParameters.subscriptionId,
            token: tokenResponse.accessToken
        });
        
        var client = azure.createResourceManagementClient(credentials);
        client.resourceGroups.list(listCallback);
        */
        console.log(tokenResponse.accessToken);
        services.getGroups(tokenResponse.accessToken, function(data){
            res.send(data);
        })
    }
  });
});

app.get('/groups', function (req, res) {
  res.send('["paul", "john"]');
});

app.listen(port, function () {
  console.log('Example app listening on port 3000!');
});