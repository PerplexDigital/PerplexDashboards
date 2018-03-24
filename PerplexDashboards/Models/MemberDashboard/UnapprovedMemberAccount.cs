using System;
using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;

namespace PerplexDashboards.Models.MemberDashboard
{
    public class UnapprovedMemberAccount : BaseMemberAccount<UnapprovedMemberAccount>
    {
        public DateTime CreateDate => Member.CreateDate;

        public UnapprovedMemberAccount(IMember member) : base(member)
        {
        }

        public static IEnumerable<UnapprovedMemberAccount> GetAll(IMemberService memberService)
        {
            return GetAll(memberService, im => !im.IsApproved, im => new UnapprovedMemberAccount(im));
        }
    }
}