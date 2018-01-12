using System;
using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;

namespace PerplexDashboards.Models.MemberDashboard
{
    public class LockedMemberAccount : BaseMemberAccount<LockedMemberAccount>
    {       
        public DateTime LastLoginDate => Member.LastLoginDate;

        public static IEnumerable<LockedMemberAccount> GetAll(DatabaseContext dbCtx)
        {
            return GetAll(dbCtx, new Sql()
                .Select("d.contentNodeId as MemberId")
                .From("cmsPropertyData d", "cmsPropertyType p")
                .Where("d.propertytypeid = p.id AND p.UniqueId = '0000001F-0000-0000-0000-000000000000' AND d.dataint = 1")
                .SQL);
        }
    }
}