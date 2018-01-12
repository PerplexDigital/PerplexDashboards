angular.module("umbraco").service("perplexRenderPropertyService", [
    "contentTypeResource",
    "dataTypeResource",
    "$q",
    function (contentTypeResource, dataTypeResource, $q) {
        /**
         * Cache of promises in progress and/or completed        
         */
        var cache = {}; // alias => name => promise

        /**
         * Returns the scaffold for a property editor with the given alias.
         * If a name is given, will only return the scaffold for the datatype
         * if it also has that name (in addition to the alias).
         * @param {any} alias Alias of the property editor
         * @param {string} [name] Name of the datatype
         * @returns {Promise}
         */
        function getPropertyTypeScaffold(alias, name) {
            var def = $q.defer();

            if (cache[alias] == null) {
                cache[alias] = {};
            }

            var cached = cache[alias][name];
            if (cached != null) {
                // Return a promise that is resolved with a
                // copy of the original promise return value
                cached.then(function (value) {
                    def.resolve(angular.copy(value));
                }, def.reject);

                return def.promise;
            }

            dataTypeResource.getAll().then(function (response) {
                // There can be multiple datatypes with the same property editor alias
                // (e.g., multiple custom listviews which all have Umbraco.ListView as alias)
                var dataTypes = _.filter(response, { alias: alias });

                var dataTypesPromise = $q.defer();

                if (dataTypes.length > 0) {
                    dataTypesPromise.resolve(dataTypes);
                } else {
                    // There is also something called 'grouped datatypes' which contains groups like 'lists',
                    // for example the Umbraco.ListView. Somehow, the built-in datatypes (e.g. List view - Content) are hidden here
                    // and are not returned by dataTypeResource.getAll() for some reason (???).
                    // Anyway, we will look there too when we cannot find the alias in the getAll().
                    dataTypeResource.getGroupedDataTypes().then(function (response) {
                        // This response contains keys of categories which point to an Array of datatypes
                        // First join them together in a big Array
                        var allTypes = _.flatten(_.values(response));
                        var dataTypes = _.filter(allTypes, { alias: alias });

                        if (dataTypes.length > 0) {
                            // Success
                            dataTypesPromise.resolve(dataTypes);
                        } else {
                            // Still nothing
                            dataTypesPromise.reject("No property editor with alias `" + alias + "` was found.");
                        }
                    }, def.reject);
                }

                dataTypesPromise.promise.then(function (dataTypes) {
                    // Optionally filter by name
                    var targetTypes = name != null ? _.filter(dataTypes, { name: name }) : dataTypes;

                    // Take the first remaining element
                    var dataType = targetTypes[0];

                    if (dataType == null) {
                        var error =
                            "None of the datatypes with alias `" + alias + "`" + "have a name equal to `" + name + "`";

                        def.reject(error);
                        return;
                    }

                    contentTypeResource.getPropertyTypeScaffold(dataType.id).then(function (propertyType) {
                        def.resolve(propertyType);
                    }, def.reject);
                }, def.reject);
            }, def.reject);

            cache[alias][name] = def.promise;
            return def.promise;
        }

        this.getPropertyTypeScaffold = getPropertyTypeScaffold;
    }
]);
