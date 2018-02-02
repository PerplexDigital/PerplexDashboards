angular.module("umbraco").service("Perplex.UserDashboard.Api", [
    "$http",
    function($http) {
        var API_ROOT = (this.API_ROOT = "/umbraco/backoffice/api/userdashboardapi/");

        this.GetViewModel = function() {
            return get("GetViewModel");
        };

        this.GetEmailSettings = function() {
            return get("GetEmailSettings");
        };

        this.Search = function(filters, timeout) {
            return post("Search", filters, timeout);
        };

        this.GetPasswordPolicy = function() {
            return get("GetPasswordPolicy");
        };

        this.SaveEmailSettings = function(settings) {
            return post("SaveEmailSettings", settings);
        };

        function get(name, params) {
            return $http.get(API_ROOT + name);
        }

        function post(name, args, timeout) {
            return $http.post(API_ROOT + name, args, { timeout: timeout && timeout.promise });
        }
    }
]);
