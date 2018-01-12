angular.module("umbraco").controller("Perplex.List.Controller", [
    "$http", "$location",
    function($http, $location) {
        var vm = this;

        var state = (vm.state = {
            search: {
                filters: {
                    Page: 1,
                    PageSize: 10,
                    Search: ""
                },

                results: {
                    items: [],
                    paging: {
                        Page: 0,
                        TotalPages: 0,
                        TotalResults: 0,
                        Pagination: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
                    }
                }
            },

            columns: [
                {
                    name: "Name",
                    property: "Name"                 
                },

                {
                    name: "Age",
                    property: "Age"
                },

                {
                    name: "asdfas",
                    property: "Name"
                },

                {
                    name: "Namwerwere",
                    property: "Name"
                }
            ],

            commands: [
                {
                    name: "Disable",
                    icon: "icon-disable",
                    fn: function(items) {
                        var ids = items.map(item => item.Id);
                        console.log("ids", ids);
                    }
                }
            ]
        });

        var fn = (vm.fn = {
            init: function() {
                fn.search();
            },

            search: function() {
                var items = [];

                for (var i = 1; i <= 100; i++) {
                    items.push({
                        Id: i,
                        Name: "Person " + i,
                        Age: i
                    });
                }

                var pagedItems = [];

                var page = state.search.filters.Page;
                var pageSize = state.search.filters.PageSize;
                for (var i = (page - 1) * pageSize; i < page * pageSize; i++) {
                    var item = items[i];

                    var re = /./;

                    try {
                        re = new RegExp(state.search.filters.Search, "i");
                    } catch (error) {}

                    if (re.test(item.Name)) {
                        pagedItems.push(items[i]);
                    }
                }

                state.search.results.items = pagedItems;
                state.search.results.paging.Page = state.search.filters.Page;
                state.search.results.paging.TotalPages = Math.ceil(
                    items.length / state.search.filters.PageSize
                );
            },

            gotoPage: function(page) {
                state.search.filters.Page = page;
                fn.search();
            }
        });

        fn.init();
    }
]);
