using PerplexDashboards.Models.UserDashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;
using Umbraco.Core.Logging;

namespace PerplexDashboards.Code.Migrations
{
    [Migration("1.0.1", 1, UserLogItem.TableName)]
    public class UserLogItemUsernameColumnMigration : MigrationBase
    {
        public UserLogItemUsernameColumnMigration(ISqlSyntaxProvider sqlSyntax, ILogger logger) 
          : base(sqlSyntax, logger) { }

        public override void Down()
        {
            Delete.Column(nameof(UserLogItem.Username)).FromTable(UserLogItem.TableName);
        }

        public override void Up()
        {
            Alter.Table(UserLogItem.TableName).AddColumn(nameof(UserLogItem.Username)).AsString().Nullable();
        }
    }
}
