using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Security;

namespace PerplexDashboards.Models.UserDashboard
{
    public class UserPasswordPolicy : PasswordPolicyBase
    {
        public bool ForgotPasswordLinkAvailable { get; set; }
        public int? UmbracoTimeoutInMinutes { get; set; }

        public UserPasswordPolicy() { }
        public UserPasswordPolicy(MembershipProviderBase provider, IUmbracoSettingsSection umbracoSettings) : base(provider)
        {
            if(umbracoSettings != null)
            {
                ForgotPasswordLinkAvailable = umbracoSettings.Security.AllowPasswordReset;
            }

            if(int.TryParse(ConfigurationManager.AppSettings["umbracoTimeOutInMinutes"], out int timeout))
            {
                UmbracoTimeoutInMinutes = timeout;
            }
        }
    }
}
