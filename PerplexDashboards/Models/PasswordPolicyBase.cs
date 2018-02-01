using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerplexDashboards.Models
{
    public abstract class PasswordPolicyBase
    {
        public int PasswordHistory { get; set; }
        public int? MaximumPasswordPage { get; set; }
        public int MinimumPasswordLength { get; set; }
        public int MinimumNonAlphaNumericCharacters { get; set; }
        public bool ForgotPasswordLinkAvailable { get; set; }
    }
}
