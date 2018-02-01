angular.module("umbraco").controller("Perplex.UserPasswordPolicy.Controller", [
    "perplexUserDashboardApi",
    function(perplexUserDashboardApi) {
        var vm = this;

        var state = (vm.state = {
            isLoading: true,

            passwordPolicy: null
        });

        var fn = (vm.fn = {
            init: function() {
                state.isLoading = true;

                perplexUserDashboardApi
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
                switch (name) {
                    case "From":
                        return function() {
                            return fn.parseDate(state.search.filters.From);
                        };
                    case "To":
                        return function() {
                            return fn.parseDate(state.search.filters.To);
                        };
                }
            },

            setValue: function(name) {
                switch (name) {
                    case "From":
                        return function(value) {
                            state.search.filters.From = value;
                        };
                    case "To":
                        return function(value) {
                            state.search.filters.To = value;
                        };
                }
            }
        });
    }
]);
