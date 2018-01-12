using System;

namespace PerplexDashboards.Models.UserDashboard
{
    public class Filters
    {
        public int? UserId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }
}