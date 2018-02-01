// DK | 2017-11-24
// Provides PerplexMap, a Proxy for either ES6 built-in Map
// or an alternative custom implementation which uses Arrays.
// Only has methods .get() / .set() / .delete().
angular.module("umbraco").factory("PerplexMap", [
    function() {
        function isFn(obj) {
            return typeof obj === "function";
        }

        function supportsMap() {
            var map = window.Map;
            return isFn(map) && isFn(map.prototype.get) && isFn(map.prototype.set) && isFn(map.prototype.delete);
        }

        // Generator for ArrayMap - only executed when needed
        function getArrayMap() {
            // Simulation of Map based on 2 Arrays.
            function ArrayMap() {
                this.keys = [];
                this.values = [];
            }

            ArrayMap.prototype = {
                get: function(key) {
                    var idx = this.keys.indexOf(key);
                    return this.values[idx];
                },

                set: function(key, value) {
                    var idx = this.keys.indexOf(key);
                    if (idx === -1) {
                        idx = this.keys.push(key) - 1;
                    }
                    this.values[idx] = value;
                },

                delete: function(key) {
                    var idx = this.keys.indexOf(key);
                    if (idx !== -1) {
                        this.keys.splice(idx, 1);
                        this.values.splice(idx, 1);
                    }
                }
            };

            return ArrayMap;
        }

        var ProxyMap = (function getProxyMap() {
            function ProxyMap() {
                // Implementation instance
                this.map;

                if (supportsMap()) {
                    // Built-in Map support
                    this.map = new Map();
                } else {
                    // Using custom ArrayMap
                    var ArrayMap = getArrayMap();
                    this.map = new ArrayMap();
                }
            }

            // Instance methods
            ProxyMap.prototype = {
                // Subset of ES6 Map API

                get: function(key) {
                    return this.map.get(key);
                },

                set: function(key, value) {
                    return this.map.set(key, value);
                },

                delete: function(key) {
                    return this.map.delete(key);
                }
            };

            return ProxyMap;
        })();

        return ProxyMap;
    }
]);
