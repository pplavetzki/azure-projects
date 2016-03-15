/* recommended */

var directivesModule = require('./index');
var FileUploader = FileUploader;

function fileUploader($parse) {
    var directive = {
        link: link,
        restrict: 'EA'
    };
    return directive;

    function link(scope, element, attrs) {
        var model = $parse(attrs.pssFileUploader);
        var modelSetter = model.assign;

        element.bind('change', function () {
            scope.$apply(function () {
                var filer = element[0].files[0];
                if (filer.type == 'image/png' || filer.type == 'image/jpeg' || filer.type == 'image/gif') {
                    document.getElementById('fileText').value = element[0].files[0].name;
                    scope.vm.fileToUpload = element[0].files[0];
                }
            });
        });
    }
}

directivesModule.directive('pssFileUploader', ['$parse', fileUploader]);