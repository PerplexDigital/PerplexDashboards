using System;
using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace PerplexDashboards.Models.MemberDashboard
{
    public class UnapprovedMemberAccount : BaseMemberAccount<UnapprovedMemberAccount>
    {
        public DateTime CreateDate => Member.CreateDate;

        public static IEnumerable<UnapprovedMemberAccount> GetAll(DatabaseContext dbCtx)
        {
            return GetAll(dbCtx, new Sql()
                .Select("d.contentNodeId as MemberId")
                .From("cmsPropertyData d", "cmsPropertyType p")
                .Where("d.propertytypeid = p.id AND p.UniqueId = '0000001E-0000-0000-0000-000000000000' AND d.dataint = 0")
                .SQL);
        }
    }
}