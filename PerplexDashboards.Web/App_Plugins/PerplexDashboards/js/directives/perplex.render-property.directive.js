angular.module("umbraco").directive("perplexRenderProperty", [
    "perplexRenderPropertyService",
    "$timeout",
    function(renderPropertyService, $timeout) {
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
                        var getValueIsFn = typeof getValue === "function";
                        var setValueIsFn = typeof setValue === "function";
                        var onChangeIsFn = typeof onChange === "function";

                        if (getValueIsFn || setValueIsFn || onChangeIsFn) {
                            var attributes = {};

                            if (getValueIsFn) {
                                attributes.get = function() {
                                    return getValue();
                                };
                            }

                            if (setValueIsFn || onChangeIsFn) {
                                attributes.set = function(value) {
                                    if (setValueIsFn) {
                                        setValue(value);
                                    }

                                    if (onChangeIsFn) {
                                        onChange(value);
                                    }
                                };
                            }

                            Object.defineProperty($scope.property, "value", attributes);
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
