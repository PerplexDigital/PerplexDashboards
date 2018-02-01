angular.module("umbraco").controller("PerplexUserDashboardController", [
    "perplexUserDashboardApi",
    "notificationsService",
    "$location",
    function(perplexUserDashboardApi, notificationsService, $location) {
        var vm = this;

        var state = (vm.state = {
            // List of { Key: ..., Value: ... }, with Key being the integer value of the AuditEvent enum and Value being the AuditEvent string representation
            events: [],

            search: {
                filters: {
                    Page: 1,
                    PageSize: 0,
                    UserId: null,
                    From: null,
                    To: null,
                    Event: null
                },

                results: {
                    Items: [],
                    Paging: {
                        Page: 0,
                        PageSize: 0,
                        TotalPages: 0,
                        TotalResults: 0,
                        Pagination: []
                    }
                }
            },

            users: [],

            columns: [
                {
                    name: "User",
                    property: "User",
                    onClick: function(item) {
                        $location.url("/users/users/user/" + item.UserId + "?subview=users");
                    }
                },
                {
                    name: "Timestamp",
                    property: "Timestamp"
                },

                {
                    name: "Event",
                    property: "Event"
                },

                {
                    name: "IP Address",
                    property: "IpAddress"
                },

                {
                    name: "Affected User",
                    property: "AffectedUser",
                    onClick: function(item) {
                        if (item.AffectedUserId == -1) {
                            return;
                        }

                        $location.url("/users/users/user/" + item.AffectedUserId + "?subview=users");
                    }
                }
            ]
        });

        var fn = (vm.fn = {
            init: function() {
                state.isLoading = true;

                perplexUserDashboardApi
                    .GetViewModel()
                    .then(function(response) {
                        var viewModel = response.data;
                        if (viewModel == null) {
                            return;
                        }

                        state.search.filters = viewModel.Filters;
                        state.search.results = viewModel.SearchResults;

                        state.events = viewModel.Events;
                        state.users = viewModel.Users;
                    })
                    .always(function() {
                        state.isLoading = false;
                    });
            },

            gotoPage: function(page) {
                state.search.filters.Page = page;
                fn.search();
            },

            search: function(page) {
                state.isLoading = true;

                if (page != null) {
                    state.search.filters.Page = page;
                }

                perplexUserDashboardApi
                    .Search(state.search.filters)
                    .then(function(response) {
                        state.search.results = response.data;
                    }, fn.onError)
                    .always(function() {
                        state.isLoading = false;
                    });
            },

            onError: function(error) {
                notificationsService.error("Error", getErrorMessage(error));
            },

            getErrorMessage: function(error) {
                if (typeof error.data === "string") return error.data;
                return error.data.Message || error.statusText;
            }
        });
    }
]);
