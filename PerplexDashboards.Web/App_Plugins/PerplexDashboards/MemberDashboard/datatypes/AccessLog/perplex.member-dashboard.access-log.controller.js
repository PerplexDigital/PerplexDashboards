angular.module("umbraco").controller("Perplex.MemberDashboard.AccessLog.Controller", [
    "Perplex.MemberDashboard.Api",
    "notificationsService",
    "$location",
    "$timeout",
    "$scope",
    "$routeParams",
    function(memberDashboardApi, notificationsService, $location, $timeout, $scope, $routeParams) {
        var vm = this;

        var state = (vm.state = {
            // List of { Key: ..., Value: ... }, with Key being the integer value of the MemberAuditAction enum and Value being the MemberAuditAction string representation
            actions: [],

            search: {
                filters: {
                    Page: 1,
                    PageSize: 0,
                    MemberId: null,
                    UserId: null,
                    From: null,
                    To: null,
                    Action: null
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

            members: [],

            columns: [
                {
                    name: "Member",
                    property: "MemberName",
                    onClick: function(item) {
                        $location.url("/member/member/edit/" + item.MemberId);
                    }
                },
                {
                    name: "User",
                    property: "UserName",
                    onClick: function(item) {
                        $location.url("/users/users/user/" + item.UserId + "?subview=users");
                    }
                },
                {
                    name: "Action",
                    property: "Action"
                },
                {
                    name: "Date & Time",
                    property: "Timestamp"
                },

                {
                    name: "IP Address",
                    property: "IpAddress"
                }
            ],

            // Debounce timeout
            timeout: 333,

            // Result of $timeout
            timer: null,

            isLoading: true
        });

        var fn = (vm.fn = {
            init: function() {
                var memberId = fn.isMemberPage() ? $routeParams.id : null;
                fn.getViewModel(memberId);
            },

            isMemberPage: function() {
                return (
                    $routeParams.section === "member" &&
                    $routeParams.tree === "member" &&
                    $routeParams.id != null &&
                    $routeParams.id.length === 36
                );
            },

            logView: function(memberId) {
                memberDashboardApi.LogMemberView(memberId);
            },

            getViewModel: function(memberId) {
                state.isLoading = true;

                memberDashboardApi
                    .GetAccessLogViewModel(memberId)
                    .then(function(response) {
                        var viewModel = response.data;
                        if (viewModel == null) {
                            return;
                        }

                        state.search.filters = viewModel.Filters;
                        state.search.results = viewModel.SearchResults;

                        state.actions = viewModel.Actions;
                        state.members = viewModel.Members;
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

                memberDashboardApi
                    .SearchAccessLog(state.search.filters)
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

        // Log view when we are on a member page now
        if (fn.isMemberPage()) {
            fn.logView($routeParams.id);

            // Also remove the "Member"-column
            state.columns = state.columns.slice(1);
        }
    }
]);
