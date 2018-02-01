angular.module("umbraco").service("PerplexMembersDashboard.ApiService", [
    "$http",
    "umbRequestHelper",
    function($http, umbRequestHelper) {
        var API_ROOT = "/umbraco/backoffice/api/MembersDashboardApi";

        this.getLockedMembers = function() {
            return $http.get(API_ROOT + "/GetLockedMembers");
        };

        this.getLockedMembersListView = function(parentId, options) {
            return getUmbracoListViewPromise(API_ROOT + "/GetLockedMembersListView", parentId, options);
        };

        this.unlockMember = function(memberId) {
            return $http.get(API_ROOT + "/UnlockMember?memberId=" + memberId);
        };

        this.getUnapprovedMembersListView = function(parentId, options) {
            return getUmbracoListViewPromise(API_ROOT + "/GetUnapprovedMembersListView", parentId, options);
        };

        this.approveMember = function(memberId) {
            return $http.get(API_ROOT + "/ApproveMember?memberId=" + memberId);
        };

        this.deleteMember = function(memberId) {
            return $http.post(API_ROOT + "/DeleteMember?memberId=" + memberId);
        };

        this.getMembersLogListView = function(parentId, options) {
            return getUmbracoListViewPromise(API_ROOT + "/GetMembersLogListView", parentId, options);
        };

        this.GetPasswordPolicy = function() {
            return $http.get(API_ROOT + "/GetPasswordPolicy");
        };

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
