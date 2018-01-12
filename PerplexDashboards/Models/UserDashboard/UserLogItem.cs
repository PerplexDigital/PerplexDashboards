using System;
using System.Collections.Generic;
using Umbraco.Core.Auditing;
using Umbraco.Core;
using System.Data.SqlClient;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;
using PerplexDashboards.Code;

namespace PerplexDashboards.Models.UserDashboard
{
    [TableName(TableName)]
    [PrimaryKey(nameof(Id), autoIncrement = true)]
    public class UserLogItem
    {
        public const string TableName = "perplexUmbracoUserLog";

        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        // PetaPoco does not support int? type...
        public int UserId { get; set; }
        // PetaPoco does not support int? type...
        public int AffectedUserId { get; set; }

        public int AuditEvent
        {
            get => (int)Event;
            set => Event = (AuditEvent)value;
        }

        [Ignore]
        public AuditEvent Event { get; set; }
        public string IpAddress { get; set; }
        public DateTime Timestamp { get; set; }

        public UserLogItem()
        {
        }

        public UserLogItem(int userId, int affectedUserId, AuditEvent @event, string ip, DateTime timestamp)
        {
            UserId = userId;
            AffectedUserId = affectedUserId;
            Event = @event;
            IpAddress = ip;
            Timestamp = timestamp;
        }

        // Restore from DB
        private UserLogItem(SqlDataReader reader)
        {
            Id = reader.GetValue(nameof(Id), reader.GetInt32);
            UserId = reader.GetValue(nameof(UserId), reader.GetInt32);
            AffectedUserId = reader.GetValue(nameof(AffectedUserId), reader.GetInt32);
            Event = reader.GetValue(nameof(Event), i => (AuditEvent) reader.GetInt32(i));
            IpAddress = reader.GetValue(nameof(IpAddress), reader.GetString);
            Timestamp = reader.GetValue(nameof(Timestamp), i => reader.GetDateTime(i).ToUniversalTime()); 
        }

        public void Save(DatabaseContext databaseContext)
        {
            DatabaseContext dbCtx = databaseContext ?? ApplicationContext.Current.DatabaseContext;

            databaseContext.Database.Insert(this);            
        }

        public static IList<UserLogItem> GetAll(DatabaseContext databaseContext = null)
        {
            DatabaseContext dbCtx = databaseContext ?? ApplicationContext.Current.DatabaseContext;

            Sql sql = new Sql()
                .Select("*")
                .From<UserLogItem>(dbCtx.SqlSyntax);                

            return dbCtx.Database.Fetch<UserLogItem>(sql);            
        }
    }
}