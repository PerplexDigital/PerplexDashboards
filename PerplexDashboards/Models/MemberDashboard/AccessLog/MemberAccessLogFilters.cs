using PerplexDashboards.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace PerplexDashboards.Models.MemberDashboard.AccessLog
{
    public class MemberAccessLogFilters : FiltersBase
    {
        public Guid? MemberId { get; set; }
        public int? UserId { get; set; }
        public MemberAccessAction? Action { get; set; }
        public string IpAddress { get; set; }
    }
}
