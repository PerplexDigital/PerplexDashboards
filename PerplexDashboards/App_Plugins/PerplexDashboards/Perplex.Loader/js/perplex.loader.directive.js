angular.module('umbraco')
.directive('perplexLoader', function () {
    return {
        restrict: "E",
        scope: {
            show: '=',
            text: '@'
        },

        templateUrl: '/App_Plugins/PerplexDashboards/Perplex.Loader/html/perplex.loader.html'
    }
});