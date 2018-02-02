using PerplexDashboards.Code;
using PerplexDashboards.Models;
using System;
using Umbraco.Core.Auditing;

namespace PerplexDashboards.Models.MemberDashboard
{
    public class MemberFilters : FiltersBase
    {
        public int? MemberId { get; set; }        
        public MemberAuditAction? Action { get; set; }
        public string IpAddress { get; set; }        
    }
}