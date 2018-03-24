using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;

namespace PerplexDashboards.Models.MemberDashboard
{
    public class LockedMemberAccount : BaseMemberAccount<LockedMemberAccount>
    {
        public DateTime LastLoginDate => Member.LastLoginDate;

        public LockedMemberAccount(IMember member) : base(member)
        {            
        }        

        public static IEnumerable<LockedMemberAccount> GetAll(IMemberService memberService)
        {
            return GetAll(memberService, im => im.IsLockedOut, im => new LockedMemberAccount(im));
        }
    }
}