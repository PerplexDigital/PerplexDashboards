using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Security;

namespace PerplexDashboards.Models.UserDashboard
{
    public class UserPasswordPolicy : PasswordPolicyBase
    {
        public bool ForgotPasswordLinkAvailable { get; set; }
        public int? UmbracoTimeoutInMinutes { get; set; }
        public bool DisableAlternativeTemplates { get; set; }
        public bool DisableFindContentById { get; set; }
        public bool UmbracoUseSSL { get; set; }

        public int MaxRequestLength { get; set; }
        public int MaxAllowedContentLength { get; set; }

        public int MaxRequestLengthUmbraco { get; set; }
        public int MaxAllowedContentLengthUmbraco { get; set; }

        public UserPasswordPolicy() { }
        public UserPasswordPolicy(MembershipProviderBase provider, IUmbracoSettingsSection umbracoSettings) : base(provider)
        {
            if(umbracoSettings != null)
            {
                ForgotPasswordLinkAvailable = umbracoSettings.Security.AllowPasswordReset;
                DisableAlternativeTemplates = umbracoSettings.WebRouting.DisableAlternativeTemplates;
                DisableFindContentById = umbracoSettings.WebRouting.DisableFindContentByIdPath;
            }

            if(int.TryParse(ConfigurationManager.AppSettings["umbracoTimeOutInMinutes"], out int timeout))
            {
                UmbracoTimeoutInMinutes = timeout;
            }

            if (bool.TryParse(ConfigurationManager.AppSettings["umbracoUseSSL"], out bool umbracoUseSSL))
            {
                UmbracoUseSSL = umbracoUseSSL;
            }

            if (ConfigurationManager.GetSection("system.web/httpRuntime") is HttpRuntimeSection httpRuntimeSection)
            {
                MaxRequestLength = httpRuntimeSection.MaxRequestLength;
            }

            // TODO: MaxAllowedContentLength
            // TODO: MaxRequestLengthUmbraco
            // TODO: MaxAllowedContentLengthUmbraco
        }
    }
}
