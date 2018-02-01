using System;
using Umbraco.Core.Auditing;

namespace PerplexDashboards.Models.UserDashboard
{
    public class Filters
    {
        public int? UserId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public AuditEvent? Event { get; set; }
        public string IpAddress { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }
}