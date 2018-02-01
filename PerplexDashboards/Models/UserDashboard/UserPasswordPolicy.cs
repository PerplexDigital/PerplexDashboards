using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Security;

namespace PerplexDashboards.Models.UserDashboard
{
    public class UserPasswordPolicy : PasswordPolicyBase
    {
        public UserPasswordPolicy() { }
        public UserPasswordPolicy(MembershipProviderBase provider) : base(provider) { }
    }
}
