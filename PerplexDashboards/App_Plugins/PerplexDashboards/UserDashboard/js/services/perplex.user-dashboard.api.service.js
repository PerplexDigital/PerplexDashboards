angular.module("umbraco").service("perplexUserDashboardApi", [
    "$http",
    function($http) {
        this.API_ROOT = "/umbraco/backoffice/api/userdashboardapi/";

        this.GetViewModel = function() {
            return get("GetViewModel");
        };

        this.Search = function(filters, timeout) {
            return post("Search", filters, timeout);
        };

        function get(name, params) {
            return $http.get(this.API_ROOT + name);
        }

        function post(name, args, timeout) {
            return $http.post(this.API_ROOT + name, args, { timeout: timeout && timeout.promise });
        }
    }
]);
