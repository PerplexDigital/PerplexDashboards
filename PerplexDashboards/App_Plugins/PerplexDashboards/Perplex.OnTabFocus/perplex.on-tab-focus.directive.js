// DK | 2017-11-24
// Directive to run functions once or every time the tab that contains the element with this attribute is activated.
// Usage: perplex-on-tab-focus="{ once: ..., always: ... }".
// Value can be a function (e.g., vm.fn.init) or an expression (e.g., 'vm.state.x = vm.state.x + 1', note the quotes).
// Typical scenario: lazily initialize a custom property editor on a background tab:
// <div ng-controller="Perplex.CustomProperty.Controller as vm" perplex-on-tab-focus="{ once: vm.fn.init }">
angular.module("umbraco").directive("perplexOnTabFocus", [
    "$parse",
    "perplexOnTabFocusService",
    function($parse, service) {
        return {
            restrict: "A",
            link: link
        };

        /**
         * Returns true if the given is a Function or string.
         * @param {*} obj Object to validate
         */
        function isFnOrString(obj) {
            var type = typeof obj;
            return type === "function" || type === "string";
        }

        /**
         * Creates a function from the given value when it is either a Function or a string.
         * @param {Function|string} value
         * @returns {Function}
         */
        function createFn(value, $scope) {
            var type = typeof value;
            if (type === "function") {
                // Already a function
                return value;
            } else if (type === "string") {
                // Expression
                return function() {
                    $parse(value)($scope);
                };
            } else {
                // Unsupported
                return null;
            }
        }

        function link($scope, $element, attr) {
            var fnObj = $parse(attr.perplexOnTabFocus)($scope);

            if (
                Object.prototype.toString.call(fnObj) !== "[object Object]" ||
                (!isFnOrString(fnObj.once) && !isFnOrString(fnObj.always))
            ) {
                throw new Error(
                    "Perplex.OnTabFocus: Expected an object shaped { once: fn/string, always: fn/string }. Got: '" +
                        attr.perplexOnTabFocus +
                        "'. Specify at least 1 of the 2 properties."
                );
            }

            var onceFn = createFn(fnObj.once, $scope);
            var alwaysFn = createFn(fnObj.always, $scope);

            // Wait for current render to finish, then subscribe
            setTimeout(function() {
                service.subscribe($scope, $element, onceFn, alwaysFn);
            });
        }
    }
]);
