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
    }
}