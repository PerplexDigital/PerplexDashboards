angular.module("umbraco").directive("perplexPagedListview", [
    function() {
        return {
            scope: {
                items: "=",
                paging: "=",
                commands: "=",
                columns: "=",
                gotoPage: "&"
            },
            restrict: "E",
            replace: true,
            templateUrl: "/App_Plugins/PerplexDashboards/html/perplex.paged-listview.html",

            controller: function($scope) {
                $scope.gotoPage = $scope.gotoPage();

                $scope.selected = [];

                $scope.selectItem = function(item) {
                    // No commands available, no selection will happen
                    if ($scope.commands == null || $scope.commands.length === 0) {
                        return;
                    }

                    var idx = $scope.selected.indexOf(item);

                    if (idx > -1) {
                        $scope.selected.splice(idx, 1);
                    } else {
                        $scope.selected.push(item);
                    }
                };

                $scope.isSelected = function(item) {
                    return $scope.selected.indexOf(item) > -1;
                };

                $scope.runCommand = function(command) {
                    command.fn($scope.selected);
                };
            }
        };
    }
]);
