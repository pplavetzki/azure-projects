/* recommended */

var directivesModule = require('./index');
var FileUploader = FileUploader;

FileUploader.$inject = ['$parse'];

function fileUploader($parse) {
    var directive = {
        link: link,
        restrict: 'EA'
    };
    return directive;

    function link(scope, element, attrs) {
        var model = $parse(attrs.fileModel);
        var modelSetter = model.assign;

        element.bind('change', function () {
            scope.$apply(function () {
                modelSetter(scope, element[0].files[0]);
            });
        });
    }
}

directivesModule.directive('pssFileUploader', fileUploader);