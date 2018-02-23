using PerplexDashboards.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Auditing;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace PerplexDashboards.Models.MemberDashboard.ActivityLog
{
    public class MemberActivityLogViewModel
    {
        public MemberFilters Filters { get; }
        public SearchResults<ApiMemberLogItem> SearchResults { get; }
        public IEnumerable<ApiMember> Members { get; }
        public IList<KeyValuePair<int, string>> Actions { get; }

        public MemberActivityLogViewModel(IMemberService memberService, DatabaseContext databaseContext, Guid? memberGuid = null)
        {
            Filters = new MemberFilters
            {
                From = DateTime.Today.AddMonths(-1),
                To = DateTime.Today,
                Page = 1,
                PageSize = 10
            };

            if(memberGuid.HasValue)
            {
                // Specific member
                IMember member = memberService.GetByKey(memberGuid.Value);

                // Only show this member's activity
                Filters.MemberId = member?.Id;
            } else
            {
                // All members
                Members = memberService
                    .GetAll(0, int.MaxValue, out int _)
                    .OrderBy(m => m.Name)
                    .Select(m => new ApiMember
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Guid = m.Key
                    });
            }

            SearchResults = ApiMemberLogItem.Search(Filters, databaseContext);

            Actions = Enums.Values<MemberAuditAction>()
                .ToDictionary(aa => (int)aa, aa => aa.GetDisplayName())
                .ToList();
        }
    }
}