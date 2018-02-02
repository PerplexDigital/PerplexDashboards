using System;
using Umbraco.Core.Auditing;

namespace PerplexDashboards.Models
{
    public class FiltersBase
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }        
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }
}