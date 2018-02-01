angular.module("umbraco").controller("Perplex.MemberPasswordPolicy.Controller", [
    "PerplexMembersDashboard.ApiService",
    function(memberDashboardApi) {
        var vm = this;

        var state = (vm.state = {
            isLoading: true,

            passwordPolicy: null
        });

        var fn = (vm.fn = {
            init: function() {
                state.isLoading = true;

                memberDashboardApi
                    .GetPasswordPolicy()
                    .then(function(response) {
                        state.passwordPolicy = response.data;
                    }, fn.onError)
                    .always(function() {
                        state.isLoading = false;
                    });
            },

            onError: function(error) {
                notificationsService.error("Error", fn.getErrorMessage(error));
            },

            getErrorMessage: function(error) {
                if (typeof error.data === "string") return error.data;
                return error.data.Message || error.statusText;
            },

            getValue: function(name) {
                return function() {
                    var value = state.passwordPolicy[name];
                    if (value == null) {
                        return value;
                    }

                    // Should be a string for the Umbraco.NoEdit datatype
                    return "" + value;
                };
            }
        });
    }
]);
