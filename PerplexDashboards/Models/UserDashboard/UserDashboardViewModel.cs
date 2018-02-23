using PerplexDashboards.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Auditing;
using Umbraco.Core.Services;

namespace PerplexDashboards.Models.UserDashboard
{
    public class UserDashboardViewModel
    {
        public UserFilters Filters { get; }
        public SearchResults<ApiUserLogItem> SearchResults { get; }
        public IEnumerable<ApiUser> Users { get; }
        public IList<KeyValuePair<int, string>> Events { get; }

        public UserDashboardViewModel(IUserService userService, DatabaseContext databaseContext = null)
        {
            Filters = new UserFilters
            {
                From = DateTime.Today.AddMonths(-1),
                To = DateTime.Today,
                Page = 1,
                PageSize = 10
            };

            SearchResults = ApiUserLogItem.Search(Filters, databaseContext);

            Users = userService
                .GetAll(0, int.MaxValue, out int _)
                .OrderBy(u => u.Name)
                .Select(u => new ApiUser
                {
                    Id = u.Id,
                    Name = u.Name
                });

            Events = Enums.Values<AuditEvent>()
                .ToDictionary(ae => (int)ae, ae => ae.GetDisplayName())
                .ToList();
        }
    }
}