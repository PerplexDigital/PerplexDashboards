using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Security;

namespace PerplexDashboards.Models
{
    public abstract class PasswordPolicyBase
    {
        public int MinRequiredPasswordLength { get; set; }
        public int MinRequiredNonAlphanumericCharacters { get; set; }
        public int MaxInvalidPasswordAttempts { get; set; }
        public bool UseLegacyEncoding { get; set; }
        public string PasswordStorageFormat { get; set; }

        // TODO
        public int PasswordHistory { get; set; }
        public int? MaximumPasswordAge { get; set; }        
        public bool ForgotPasswordLinkAvailable { get; set; }

        public PasswordPolicyBase() {}

        public PasswordPolicyBase(MembershipProviderBase provider)
        {
            MinRequiredPasswordLength = provider.MinRequiredPasswordLength;
            MinRequiredNonAlphanumericCharacters = provider.MinRequiredNonAlphanumericCharacters;
            MaxInvalidPasswordAttempts = provider.MaxInvalidPasswordAttempts;
            UseLegacyEncoding = provider.UseLegacyEncoding;
            PasswordStorageFormat = provider.PasswordFormat.ToString();
        }
    }
}
