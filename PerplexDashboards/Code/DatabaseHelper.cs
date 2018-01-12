using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;

namespace PerplexDashboards.Code
{
    public static class DatabaseHelper
    {
        public static bool DatabaseTableExists(DatabaseContext dbCtx, string tableName)
        {
            DatabaseSchemaHelper dsh = new DatabaseSchemaHelper(
                dbCtx.Database,
                ApplicationContext.Current.ProfilingLogger.Logger,
                dbCtx.SqlSyntax);

            return dsh.TableExist(tableName);
        }

        public static void CreateDatabaseTableIfNeeded<T>(DatabaseContext dbCtx, string tableName, ILogger logger = null) where T : new()
        {
            if (DatabaseTableExists(dbCtx, tableName))
            {
                return;
            }

            DatabaseSchemaHelper dsh = new DatabaseSchemaHelper(
                dbCtx.Database,
                logger ?? ApplicationContext.Current.ProfilingLogger.Logger,
                dbCtx.SqlSyntax);

            dsh.CreateTable<T>();
        }
    }
}
