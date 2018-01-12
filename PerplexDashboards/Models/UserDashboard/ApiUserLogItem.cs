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

namespace PerplexDashboards.Models.UserDashboard
{
    public class ApiUserLogItem
    {   
        public int UserId { get; set; }
        public string User { get; set; }
        public int AffectedUserId { get; set; }
        public string AffectedUser { get; set; }        
        public string Event { get; set; }
        public string IpAddress { get; set; }
        public string Timestamp { get; set; }

        public ApiUserLogItem(UserLogItem item, IUserService userService = null)
        {
            IUserService us = userService ?? ApplicationContext.Current?.Services?.UserService;
            if(us == null)
            {
                throw new NullReferenceException($"User service could not be obtained!");
            }

            UserId = item.UserId;
            User = GetUsername(UserId, us);
            AffectedUserId = item.AffectedUserId;
            AffectedUser = GetUsername(AffectedUserId, us);
            Event = item.Event.ToString();
            IpAddress = item.IpAddress;

            DateTime localTime = item.Timestamp.ToLocalTime();
            Timestamp = $"{localTime.ToShortDateString()} {localTime.ToLongTimeString()}";
        }

        private string GetUsername(int userId, IUserService userService)
        {
            const string noUser = "none";

            if (userId == -1)
            {
                return noUser;
            }            

            return userService.GetUserById(userId)?.Name ?? noUser;
        }

        public static SearchResults<ApiUserLogItem> Search(Filters filters, DatabaseContext databaseContext = null)
        {
            // Very basic filtering / pagination done here and not in DB
            List<ApiUserLogItem> all = UserLogItem.GetAll(databaseContext)
                .Where(i => !filters.UserId.HasValue || (i.UserId == filters.UserId || i.AffectedUserId == filters.UserId))
                .OrderByDescending(ia => ia.Timestamp)
                .Select(i => new ApiUserLogItem(i))                             
                .ToList();

            IEnumerable<ApiUserLogItem> page = all.Skip((filters.Page - 1) * filters.PageSize).Take(filters.PageSize);
            return new SearchResults<ApiUserLogItem>(page, all.Count, filters.Page, filters.PageSize);
        }
    }
}