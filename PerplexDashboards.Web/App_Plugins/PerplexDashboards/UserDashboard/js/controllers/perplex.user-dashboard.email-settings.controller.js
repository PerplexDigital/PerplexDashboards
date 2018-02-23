angular.module("umbraco").controller("Perplex.UserDashboard.EmailSettings.Controller", [
    "notificationsService",
    "Perplex.UserDashboard.Api",
    function(notificationsService, userDashboardApi) {
        var vm = this;

        var state = (vm.state = {
            isLoading: true,
            isSaving: false,

            propertyEditors: [
                {
                    alias: "Umbraco.Textbox",
                    label: "Locked Account - Recipient Email Address",
                    description: "The recipient of the email which is sent after a user account has been locked",
                    getValue: get("LockedEmailRecipientAddress"),
                    setValue: set("LockedEmailRecipientAddress")
                },
                {
                    alias: "Umbraco.Textbox",
                    label: "Locked Account - Email Subject",
                    description: "The subject of the email which is sent after a user account has been locked",
                    getValue: get("LockedEmailSubject"),
                    setValue: set("LockedEmailSubject")
                },
                {
                    alias: "Umbraco.TinyMCEv3",
                    label: "Locked Account - Email Body",
                    description:
                        "The body of the email which is sent after a user account has been locked. The following tags can be used in the emails:<br>" +
                        "[#username#]: The username of the locked user<br>" +
                        "[#datetime#]: The date and time that the account was locked<br>" +
                        "[#website#]: The name of the website",

                    getValue: get("LockedEmailBody"),
                    setValue: set("LockedEmailBody")
                }
            ],

            settings: {
                LockedEmailSubject: null,
                LockedEmailBody: null,
                LockedEmailRecipientAddress: null
            }
        });

        var fn = (vm.fn = {
            init: function() {
                fn.initData();
            },

            initData: function() {
                state.isLoading = true;

                userDashboardApi
                    .GetEmailSettings()
                    .then(function(response) {
                        state.settings = response.data;
                    })
                    .always(function() {
                        state.isLoading = false;
                    });
            },

            save: function() {
                state.isSaving = true;
                userDashboardApi
                    .SaveEmailSettings(state.settings)
                    .then(function() {
                        notificationsService.success("Saved", "E-mail Settings were saved");
                    })
                    .always(function() {
                        state.isSaving = false;
                    });
            }
        });

        function set(property) {
            return function(value) {
                // TinyMCE tries to set `undefined` regularly as value,
                // even though the RTE is not empty, wat.
                if (typeof value !== "undefined") {
                    state.settings[property] = value;
                }
            };
        }

        function get(property) {
            return function(alias) {
                return state.settings[property];
            };
        }
    }
]);
