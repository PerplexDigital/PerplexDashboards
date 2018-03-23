using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Auditing;
using System.Threading.Tasks;
using Umbraco.Core;
using System.Data.SqlClient;
using System.Data;
using Umbraco.Core.Services;
using Umbraco.Core.Persistence;
using System.Text.RegularExpressions;
using PerplexDashboards.Code;

namespace PerplexDashboards.Models.UserDashboard
{
    public class ApiUserLogItem
    {
        public int PerformingUserId { get; set; }
        public string PerformingUser { get; set; }
        public int AffectedUserId { get; set; }
        public string AffectedUser { get; set; }        
        public string Event { get; set; }
        public string IpAddress { get; set; }
        public string Timestamp { get; set; }

        public ApiUserLogItem(UserLogItem item, IUserService userService)
        {
            PerformingUserId = item.PerformingUserId;
            PerformingUser = string.IsNullOrEmpty(item.Username) 
                ? GetUsername(PerformingUserId, userService)
                : item.Username;

            AffectedUserId = item.AffectedUserId;
            AffectedUser = GetUsername(AffectedUserId, userService);
            Event = item.Event.GetDisplayName();
            IpAddress = item.IpAddress;

            DateTime localTime = item.Timestamp.ToLocalTime();
            Timestamp = $"{localTime.ToShortDateString()} {localTime.ToLongTimeString()}";
        }

        private string GetUsername(int userId, IUserService userService)
        {
            const string noUser = "";

            if (userId == -1)
            {
                return noUser;
            }            

            return userService.GetUserById(userId)?.Name ?? noUser;
        }

        public static SearchResults<ApiUserLogItem> Search(UserFilters filters, DatabaseContext databaseContext, IUserService userService)
        {          
            List<ApiUserLogItem> items = UserLogItem.GetAll(databaseContext, filters)              
                .Select(i => new ApiUserLogItem(i, userService))                             
                .ToList();

            IEnumerable<ApiUserLogItem> page = items.Skip((filters.Page - 1) * filters.PageSize).Take(filters.PageSize);
            return new SearchResults<ApiUserLogItem>(page, items.Count, filters.Page, filters.PageSize);
        }
    }
}