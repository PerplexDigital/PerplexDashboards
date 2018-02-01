using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Security;

namespace PerplexDashboards.Models.MemberDashboard
{
    public class MemberPasswordPolicy : PasswordPolicyBase
    {
        public MemberPasswordPolicy() { }

        public MemberPasswordPolicy(MembershipProviderBase provider) : base(provider) { }
    }
}
