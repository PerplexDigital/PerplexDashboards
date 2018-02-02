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

namespace PerplexDashboards.Models.MemberDashboard
{
    public class ApiMemberLogItem
    {   
        public int MemberId { get; }
        public Guid? MemberGuid { get; }
        public string MemberName { get; }
        public string Timestamp { get; }
        public string Action { get; }
        public string IpAddress { get; }

        public ApiMemberLogItem(MemberLogItem item, IMemberService memberService = null)
        {
            IMemberService ms = memberService ?? ApplicationContext.Current?.Services?.MemberService;
            if(ms == null)
            {
                throw new NullReferenceException($"User service could not be obtained!");
            }

            MemberId = item.UserId;

            IMember member = ms.GetById(MemberId);
            if(member == null)
            {
                MemberName = "none";
            } else
            {
                MemberName = member.Name;
                MemberGuid = member.Key;
            }

            Action = item.AuditAction;
            IpAddress = item.IP;

            DateTime localTime = item.Date.ToLocalTime();
            Timestamp = $"{localTime.ToShortDateString()} {localTime.ToLongTimeString()}";
        }
      
        public static SearchResults<ApiMemberLogItem> Search(MemberFilters filters, DatabaseContext databaseContext = null)
        {          
            List<ApiMemberLogItem> items = MemberLogItem.GetAll(databaseContext, filters)              
                .Select(i => new ApiMemberLogItem(i))                             
                .ToList();

            IEnumerable<ApiMemberLogItem> page = items.Skip((filters.Page - 1) * filters.PageSize).Take(filters.PageSize);
            return new SearchResults<ApiMemberLogItem>(page, items.Count, filters.Page, filters.PageSize);
        }
    }
}