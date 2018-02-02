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
using Umbraco.Core.Models;
using Umbraco.Core.Models.Membership;

namespace PerplexDashboards.Models.MemberDashboard.AccessLog
{
    public class ApiMemberAccessLogItem
    {   
        public Guid MemberId { get; }
        public string MemberName { get; }

        public int UserId { get; }
        public string UserName { get; set; }

        public string Timestamp { get; }
        public string Action { get; }
        public string IpAddress { get; }

        public ApiMemberAccessLogItem(MemberAccessLogItem item, IMemberService memberService, IUserService userService)
        {
            MemberId = item.MemberId;

            IMember member = memberService.GetByKey(MemberId);
            MemberName = member.Name ?? "none";

            UserId = item.UserId;
            IUser user = userService.GetUserById(UserId);
            UserName = user?.Name ?? "none";

            Action = item.Action;
            IpAddress = item.IpAddress;

            DateTime localTime = item.Timestamp.ToLocalTime();
            Timestamp = $"{localTime.ToShortDateString()} {localTime.ToLongTimeString()}";
        }
      
        public static SearchResults<ApiMemberAccessLogItem> Search(MemberAccessLogFilters filters, DatabaseContext databaseContext, IMemberService memberService, IUserService userService)
        {          
            List<ApiMemberAccessLogItem> items = MemberAccessLogItem.GetAll(databaseContext, filters)              
                .Select(i => new ApiMemberAccessLogItem(i, memberService, userService))                             
                .ToList();

            IEnumerable<ApiMemberAccessLogItem> page = items.Skip((filters.Page - 1) * filters.PageSize).Take(filters.PageSize);
            return new SearchResults<ApiMemberAccessLogItem>(page, items.Count, filters.Page, filters.PageSize);
        }
    }
}