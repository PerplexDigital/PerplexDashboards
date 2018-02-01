angular.module("umbraco").directive("perplexRenderProperty", [
    "perplexRenderPropertyService",
    function(renderPropertyService) {
        return {
            scope: {
                alias: "=",
                name: "=?",
                label: "=?",
                description: "=?",
                showLabel: "=?",
                getValue: "&?",
                setValue: "&?",
                onChange: "&?"
            },

            restrict: "E",
            replace: true,
            templateUrl: "/App_Plugins/PerplexDashboards/html/perplex.render-property.html",

            controller: function($scope) {
                var getValue = $scope.getValue();
                var setValue = $scope.setValue();
                var onChange = $scope.onChange();

                renderPropertyService.getPropertyTypeScaffold($scope.alias, $scope.name).then(
                    function(propertyTypeScaffold) {
                        $scope.property = propertyTypeScaffold;
                        $scope.property.hideLabel = !$scope.label && !$scope.showLabel;
                        $scope.property.label = $scope.label;
                        $scope.property.description = $scope.description;

                        // The property's value will be stored externally
                        // using get / set functions, if available
                        if (typeof getValue === "function" && typeof setValue === "function") {
                            Object.defineProperty($scope.property, "value", {
                                get: function() {
                                    return getValue();
                                },
                                set: function(value) {
                                    setValue(value);

                                    if (typeof onChange === "function") {
                                        onChange(value);
                                    }
                                }
                            });
                        }
                    },
                    function(error) {
                        throw new Error(error);
                    }
                );
            }
        };
    }
]);
