// DK | 2017-11-24
// Service which manages watching the necessary tabs in Umbraco through MutationObservers.
// Every usage of perplex-on-tab-focus yields a subscription to this service, which will
// be responsible for calling the provided callbacks (once and/or always) when the right tab becomes
// active. It also makes sure only 1 MutationObserver is instantiated per unique tab (rather than 1 MutationObserver
// per property, which could generate lots of MutationObservers for the same tab).
// Lastly, it makes sure the MutationObservers are disconnected when all subscriptions have ended.
angular.module("umbraco").service("perplexOnTabFocusService", [
    "$timeout",
    "PerplexMap",
    function($timeout, PerplexMap) {
        // Keeps track of MutationObserver and associated callbacks for one tab.
        // Format: <Element> => { fns: Array<Function>, observer: <MutationObserver> }
        var map = new PerplexMap();

        // List of Functions to be executed immediately upon subscribing.
        // We group these together so they can be executed in a single $timeout
        // rather than a $timeout for each subscription.
        var onSubscribeFns = [];

        this.subscribe = function($scope, $element, onceFn, alwaysFn) {
            var tab = $element.closest(".umb-tab-pane")[0];

            var fns = createFns(tab, onceFn, alwaysFn);
            var onTabFocusFn = fns.onTabFocusFn;
            var unsubscribeFn = fns.unsubscribeFn;

            // Unsubscribe caller from service when caller's $scope is destroyed
            $scope.$on("$destroy", unsubscribeFn);

            var supportsMutationObserver = typeof window.MutationObserver === "function";

            if (tab == null || !supportsMutationObserver) {
                // No tab to observe and/or MutationObserver is not supported at all.
                // The best we can do is just run the functions at least once, then quit silently.
                // This might occur when using a custom datatype in some other context (e.g., a Dashboard) where it is not
                // inside a umb-tab-pane.
                addOnSubscribeFn(onTabFocusFn);
                return;
            }

            // If the tab was already active MutationObserver will miss it,
            // so run right away.
            if (tabIsActive(tab)) {
                addOnSubscribeFn(onTabFocusFn);
                if (typeof alwaysFn !== "function") {
                    return;
                }
            }

            var data = map.get(tab);

            if (data == null) {
                var observer = startObserver(tab, function() {
                    // Copy of data.fns, as it might be modified from inside the for loop,
                    // via unsubscribeFn which removes functions from data.fns.
                    var fns = data.fns.slice();
                    runInTimeout(fns);
                });

                data = {
                    fns: [],
                    observer: observer
                };

                map.set(tab, data);
            }

            data.fns.push(onTabFocusFn);

            return unsubscribeFn;
        };

        /**
         * Adds a Function to be executed immediately on subscription
         * @param {Function} fn Function to add
         */
        function addOnSubscribeFn(fn) {
            onSubscribeFns.push(fn);

            if (onSubscribeFns.length === 1) {
                // Schedule execution when receiving first Function
                runInTimeout(onSubscribeFns, function() {
                    // Clear Array afterwards
                    onSubscribeFns.length = 0;
                });
            }
        }

        /**
         * Runs the given Functions inside a single $timeout function
         * @param {Array<Function>} fns Functions to run
         * @param {Function} [callback] Callback to run after all Functions are executed
         */
        function runInTimeout(fns, callback) {
            $timeout(function() {
                for (var i = 0; i < fns.length; i++) {
                    fns[i]();
                }

                if (typeof callback === "function") {
                    callback();
                }
            });
        }

        /**
         * Creates onTabFocusFn + UnsubscribeFn.
         * They depend on each other thus have to be created in the same function scope.
         * @param {*} tab DOM element
         * @param {*} onceFn Function to run once
         * @param {*} alwaysFn Function to run always
         */
        function createFns(tab, onceFn, alwaysFn) {
            // Removes the provided callbacks and disconnects the MutationObserver
            // when this was the last subscriber for the tab.
            function unsubscribeFn() {
                var data = map.get(tab);
                if (data == null) return;

                var idx = data.fns.indexOf(onTabFocusFn);
                if (idx > -1) {
                    data.fns.splice(idx, 1);
                    if (data.fns.length === 0) {
                        data.observer.disconnect();
                        data.observer = null;
                        map.delete(tab);
                    }
                }
            }

            var hasRun = false;
            function onTabFocusFn() {
                if (!hasRun) {
                    if (typeof onceFn === "function") {
                        onceFn();
                    }

                    hasRun = true;

                    if (typeof alwaysFn !== "function") {
                        unsubscribeFn();
                        return;
                    }
                }

                alwaysFn();
            }

            return { onTabFocusFn: onTabFocusFn, unsubscribeFn: unsubscribeFn };
        }

        /**
         * Returns true if the tab is currently active
         * @param {Element} tab The tab to check
         * @returns {boolean}
         */
        function tabIsActive(tab) {
            return tab.classList.contains("active");
        }

        /**
         * Returns true if the tab was activated, based on information in the given MutationRecord
         * @param {MutationRecord} mutation A MutationRecord for the tab
         * @returns {boolean}
         */
        function tabWasActivated(mutation) {
            if (!tabIsActive(mutation.target)) {
                return false;
            }

            var tabWasActive = /(^| )active( |$)/.test(mutation.oldValue);
            return !tabWasActive;
        }

        /**
         * Configures a MutationObserver to start watching the tab and call the
         * provided callback function when it becomes active.
         * @param {Element} tab The tab to observe
         * @param {Function} callback Callback to run when the tab becomes active
         * @returns {MutationObserver}
         */
        function startObserver(tab, callback) {
            var observer = new MutationObserver(function(mutations) {
                for (var i = 0; i < mutations.length; i++) {
                    var mutation = mutations[i];

                    if (tabWasActivated(mutation)) {
                        callback();
                    }
                }
            });

            // Start observing
            observer.observe(tab, {
                attributes: true,
                // Only look at changes to the "class" attribute
                attributeFilter: ["class"],
                // We need the old value to check if the tab was already active before the mutation
                attributeOldValue: true
            });

            return observer;
        }
    }
]);
