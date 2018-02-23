using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Web;
using Umbraco.Core;
using Umbraco.Web.WebApi;
using Umbraco.Core.Security;
using Umbraco.Core.Auditing;
using System.Text.RegularExpressions;

namespace PerplexDashboards.Code
{
    public static class Extensions
    {
        public static T GetValue<T>(this SqlDataReader reader, string columnName, Func<int, T> getValue)
        {
            int index = reader.GetOrdinal(columnName);
            if (index < 0 || reader.IsDBNull(index))
            {
                return default(T);
            }

            return getValue(index);
        }      

        public static string GetDisplayName(this AuditEvent evt)
        {            
            return Regex.Replace(evt.ToString(), @"([a-z])([A-Z])", m => $"{m.Groups[1]} {m.Groups[2]}");            
        }

        public static string GetDisplayName(this MemberAuditAction? action)
        {
            if(action == null)
            {
                return "";
            }

            return Regex.Replace(action.ToString(), @"([a-z])([A-Z])", m => $"{m.Groups[1]} {m.Groups[2]}");
        }

        public static string GetDisplayName(this MemberAuditAction action)
        {
            return ((MemberAuditAction?)action).GetDisplayName();
        }
    }
}