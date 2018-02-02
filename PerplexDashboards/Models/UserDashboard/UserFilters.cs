using System;
using Umbraco.Core.Auditing;

namespace PerplexDashboards.Models.UserDashboard
{
    public class UserFilters : FiltersBase
    {
        public int? UserId { get; set; }        
        public AuditEvent? Event { get; set; }
        public string IpAddress { get; set; }        
    }
}