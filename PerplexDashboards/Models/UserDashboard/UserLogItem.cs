using System;
using System.Collections.Generic;
using Umbraco.Core.Auditing;
using Umbraco.Core;
using System.Data.SqlClient;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;
using PerplexDashboards.Code;
using System.Linq.Expressions;
using Semver;
using Umbraco.Core.Models;
using System.Linq;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Logging;

namespace PerplexDashboards.Models.UserDashboard
{
    [TableName(TableName)]
    [PrimaryKey(nameof(Id), autoIncrement = true)]
    public class UserLogItem
    {
        private readonly static SemVersion TargetMigrationVersion = new SemVersion(1, 0, 2);

        public const string TableName = "perplexUmbracoUserLog";

        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        public int PerformingUserId { get; set; }        
        public int AffectedUserId { get; set; }

        [NullSetting(NullSetting = NullSettings.Null)]
        public string Username { get; set; }

        public int AuditEvent
        {
            get => (int)Event;
            set => Event = (AuditEvent)value;
        }

        [Ignore]
        public AuditEvent Event { get; set; }

        [NullSetting(NullSetting = NullSettings.Null)]
        public string IpAddress { get; set; }
        public DateTime Timestamp { get; set; }

        public UserLogItem()
        {
        }

        public UserLogItem(int performingUserId, int affectedUserId, string username, AuditEvent @event, string ip, DateTime timestamp)
        {
            PerformingUserId = performingUserId;
            AffectedUserId = affectedUserId;
            Username = username;
            Event = @event;
            IpAddress = ip;
            Timestamp = timestamp;
        }

        // Restore from DB
        private UserLogItem(SqlDataReader reader)
        {
            Id = reader.GetValue(nameof(Id), reader.GetInt32);
            PerformingUserId = reader.GetValue(nameof(PerformingUserId), reader.GetInt32);
            AffectedUserId = reader.GetValue(nameof(AffectedUserId), reader.GetInt32);
            Username = reader.GetValue(nameof(Username), reader.GetString);
            Event = reader.GetValue(nameof(Event), i => (AuditEvent) reader.GetInt32(i));
            IpAddress = reader.GetValue(nameof(IpAddress), reader.GetString);
            Timestamp = reader.GetValue(nameof(Timestamp), i => reader.GetDateTime(i).ToUniversalTime()); 
        }

        public void Save(DatabaseContext databaseContext)
        {
            databaseContext.Database.Save(this);            
        }

        public static IList<UserLogItem> GetAll(DatabaseContext databaseContext = null, UserFilters filters = null)
        {
            DatabaseContext dbCtx = databaseContext ?? ApplicationContext.Current.DatabaseContext;

            Sql sql = new Sql()
                .Select("*")
                .From<UserLogItem>(dbCtx.SqlSyntax);

            if (filters != null)
            {
                Action<Expression<Func<UserLogItem, bool>>> addWhere = 
                    expr => sql = sql.Where(expr, dbCtx.SqlSyntax);

                if(filters.From != null)
                {
                    addWhere(i => i.Timestamp >= filters.From.Value.Date);
                }
                
                if(filters.To != null)
                {
                    DateTime to = filters.To.Value.Date.AddDays(1);
                    addWhere(i => i.Timestamp < to);
                }

                if(filters.Event != null)
                {
                    addWhere(i => i.AuditEvent == (int)filters.Event);
                }

                if(filters.UserId != null)
                {
                    addWhere(i => i.PerformingUserId == filters.UserId || i.AffectedUserId == filters.UserId);
                }

                if (!string.IsNullOrEmpty(filters.IpAddress))
                {
                    sql = sql.Where($"{nameof(IpAddress)} LIKE @0", new[] { $"%{filters.IpAddress}%" });
                }
            }

            sql = sql.OrderByDescending<UserLogItem>(item => item.Timestamp, dbCtx.SqlSyntax);

            return dbCtx.Database.Fetch<UserLogItem>(sql);            
        }

        public static void RunMigrations(ApplicationContext appCtx)
        {
            IEnumerable<IMigrationEntry> migrations = appCtx.Services.MigrationEntryService.GetAll(TableName);

            SemVersion currentVersion = migrations
                .OrderByDescending(x => x.CreateDate)
                .FirstOrDefault()
                ?.Version;

            if(currentVersion == null)
            {
                // We are up to date, as we installed without a previous version being present
                appCtx.Services.MigrationEntryService.CreateEntry(TableName, TargetMigrationVersion);
                return;
            }

            if (currentVersion == TargetMigrationVersion)
            {
                // We were up to date already
                return;
            }                            

            var migrationsRunner = new MigrationRunner(
               appCtx.Services.MigrationEntryService,
               appCtx.ProfilingLogger.Logger,
               currentVersion,
               TargetMigrationVersion,
               TableName);

            try
            {
                migrationsRunner.Execute(appCtx.DatabaseContext.Database, currentVersion < TargetMigrationVersion);
            }
            catch (Exception e)
            {
                LogHelper.Error<UserLogItem>($"Error running {TableName} migration", e);
            }
        }
    }
}