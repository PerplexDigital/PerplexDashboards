using PerplexDashboards.Code;
using PerplexDashboards.Models.UserDashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Auditing;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace PerplexDashboards.Models.MemberDashboard.AccessLog
{
    public class MemberAccessLogViewModel
    {
        public MemberAccessLogFilters Filters { get; }
        public SearchResults<ApiMemberAccessLogItem> SearchResults { get; }
        public IEnumerable<ApiMember> Members { get; }
        public IEnumerable<ApiUser> Users { get; }
        public IList<KeyValuePair<int, string>> Actions { get; }

        public MemberAccessLogViewModel(IMemberService memberService, IUserService userService, DatabaseContext databaseContext, Guid? memberId = null)
        {
            Filters = new MemberAccessLogFilters
            {
                From = DateTime.Today.AddMonths(-1),
                To = DateTime.Today,
                Page = 1,
                PageSize = 10
            };

            if(memberId.HasValue)
            {
                // Only show this member's activity
                Filters.MemberId = memberId;
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

            SearchResults = ApiMemberAccessLogItem.Search(Filters, databaseContext, memberService, userService);
            Users = userService
                .GetAll(0, int.MaxValue, out int _)
                .OrderBy(u => u.Name)
                .Select(u => new ApiUser
                {
                    Id = u.Id,
                    Name = u.Name
                });

            Actions = Enums.Values<MemberAccessAction>()
                .ToDictionary(aa => (int)aa, aa => aa.ToString())
                .ToList();
        }
    }
}