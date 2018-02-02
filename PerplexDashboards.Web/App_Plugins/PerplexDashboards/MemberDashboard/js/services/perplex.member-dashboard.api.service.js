angular.module("umbraco").service("Perplex.MemberDashboard.Api", [
    "$http",
    "umbRequestHelper",
    function($http, umbRequestHelper) {
        var API_ROOT = "/umbraco/backoffice/api/MemberDashboardApi/";

        this.GetActivityLogViewModel = function(memberGuid) {
            return get("GetActivityLogViewModel?memberGuid=" + memberGuid);
        };

        this.SearchActivityLog = function(filters, timeout) {
            return post("SearchActivityLog", filters, timeout);
        };

        this.getLockedMembers = function() {
            return $http.get(API_ROOT + "GetLockedMembers");
        };

        this.getLockedMembersListView = function(parentId, options) {
            return getUmbracoListViewPromise(API_ROOT + "GetLockedMembersListView", parentId, options);
        };

        this.unlockMember = function(memberId) {
            return $http.get(API_ROOT + "UnlockMember?memberId=" + memberId);
        };

        this.getUnapprovedMembersListView = function(parentId, options) {
            return getUmbracoListViewPromise(API_ROOT + "GetUnapprovedMembersListView", parentId, options);
        };

        this.approveMember = function(memberId) {
            return $http.get(API_ROOT + "ApproveMember?memberId=" + memberId);
        };

        this.deleteMember = function(memberId) {
            return $http.post(API_ROOT + "DeleteMember?memberId=" + memberId);
        };

        this.getMembersLogListView = function(parentId, options) {
            return getUmbracoListViewPromise(API_ROOT + "GetMembersLogListView", parentId, options);
        };

        this.GetPasswordPolicy = function() {
            return $http.get(API_ROOT + "GetPasswordPolicy");
        };

        function get(name, params) {
            return $http.get(API_ROOT + name);
        }

        function post(name, args, timeout) {
            return $http.post(API_ROOT + name, args, { timeout: timeout && timeout.promise });
        }

        function getUmbracoListViewPromise(url, parentId, options) {
            var defaults = {
                pageSize: 0,
                pageNumber: 0,
                filter: "",
                orderDirection: "asc",
                orderBy: ""
            };

            if (options === undefined) {
                options = {};
            }

            //overwrite the defaults if there are any specified
            angular.extend(defaults, options);
            //now copy back to the options we will use
            options = defaults;

            return umbRequestHelper.resourcePromise(
                $http.get(url, {
                    params: {
                        id: parentId == -1 ? null : parentId,
                        pageNumber: options.pageNumber,
                        pageSize: options.pageSize,
                        orderBy: options.orderBy,
                        orderDirection: options.orderDirection,
                        filter: options.filter
                    }
                }),
                "Failed to retrieve children for URL " + url
            );
        }
    }
]);
