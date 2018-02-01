angular.module("umbraco").service("perplexUserDashboardApi", [
    "$http",
    function($http) {
        var API_ROOT = (this.API_ROOT = "/umbraco/backoffice/api/userdashboardapi/");

        this.GetViewModel = function() {
            return get("GetViewModel");
        };

        this.Search = function(filters, timeout) {
            return post("Search", filters, timeout);
        };

        function get(name, params) {
            return $http.get(API_ROOT + name);
        }

        function post(name, args, timeout) {
            return $http.post(API_ROOT + name, args, { timeout: timeout && timeout.promise });
        }
    }
]);
