using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Services;

namespace PerplexDashboards.Models.UserDashboard
{
    public class UserDashboardViewModel
    {
        public Filters Filters { get; set; }
        public SearchResults<ApiUserLogItem> SearchResults { get; set; }
        public IEnumerable<ApiUser> Users { get; set; }

        public UserDashboardViewModel(IUserService userService, DatabaseContext databaseContext = null)
        {
            Filters = new Filters
            {
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
        }
    }
}