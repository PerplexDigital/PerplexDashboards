angular.module("umbraco").controller("Perplex.UserDashboard.Controller", [
    "Perplex.UserDashboard.Api",
    "notificationsService",
    "$location",
    "$timeout",
    "$scope",
    function(userDashboardApi, notificationsService, $location, $timeout, $scope) {
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
                    name: "Date & Time",
                    property: "Timestamp"
                },

                {
                    name: "Event",
                    property: "Event"
                },

                {
                    name: "Affected User",
                    property: "AffectedUser",
                    skipOnClick: function (item) {
                        return item.AffectedUserId === -1;
                    },
                    onClick: function (item) {
                        $location.url("/users/users/user/" + item.AffectedUserId + "?subview=users");
                    }
                },   

                {
                    name: "Performing User",
                    property: "PerformingUser",
                    skipOnClick: function (item) {
                        return item.PerformingUserId === -1;
                    },
                    onClick: function (item) {
                        $location.url("/users/users/user/" + item.PerformingUserId + "?subview=users");
                    }
                },
              
                {
                    name: "IP Address",
                    property: "IpAddress"
                }               
            ],

            // Debounce timeout
            timeout: 444,

            // Result of $timeout
            timer: null,

            isLoading: true
        });

        var fn = (vm.fn = {
            init: function() {
                state.isLoading = true;

                userDashboardApi
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

            search: function(page, useTimeout) {
                if (useTimeout) {
                    if (state.timer != null) {
                        $timeout.cancel(state.timer);
                    }

                    state.timer = $timeout(function() {
                        fn.search(page);
                    }, state.timeout);

                    return;
                }

                state.search.isLoading = true;

                if (page != null) {
                    state.search.filters.Page = page;
                }

                userDashboardApi
                    .Search(state.search.filters)
                    .then(function(response) {
                        state.search.results = response.data;
                    }, fn.onError)
                    .always(function() {
                        state.search.isLoading = false;
                    });
            },

            onError: function(error) {
                notificationsService.error("Error", fn.getErrorMessage(error));
            },

            getErrorMessage: function(error) {
                if (typeof error.data === "string") return error.data;
                return error.data.Message || error.statusText;
            },

            parseDate: function(date) {
                if (/^\d{4}-\d{2}-\d{2}$/.test(date)) {
                    return date;
                }

                if (/^\d{4}-\d{2}-\d{2}/.test(date)) {
                    return date.substr(0, 10);
                }

                return null;
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
            },

            onChange: function() {
                fn.search(1);
            }
        });
    }
]);
