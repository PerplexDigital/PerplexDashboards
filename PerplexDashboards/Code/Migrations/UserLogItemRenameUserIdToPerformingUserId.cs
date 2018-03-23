using PerplexDashboards.Models.UserDashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;

namespace PerplexDashboards.Code.Migrations
{
    [Migration("1.0.2", 1, UserLogItem.TableName)]
    public class UserLogItemRenameUserIdToPerformingUserId : MigrationBase
    {
        private string OldName = "UserId";
        private string NewName = "PerformingUserId";

        public UserLogItemRenameUserIdToPerformingUserId(ISqlSyntaxProvider sqlSyntax, ILogger logger) 
          : base(sqlSyntax, logger) { }

        public override void Down()
        {
            IfDatabase(DatabaseProviders.SqlServerCE)
                .Create.Column(OldName).OnTable(UserLogItem.TableName).AsInt32().Nullable();

            Rename.Column(NewName).OnTable(UserLogItem.TableName).To(OldName);

            IfDatabase(DatabaseProviders.SqlServerCE)
                .Delete.Column(NewName).FromTable(UserLogItem.TableName);
        }

        public override void Up()
        {
            IfDatabase(DatabaseProviders.SqlServerCE)
                .Create.Column(NewName).OnTable(UserLogItem.TableName).AsInt32().Nullable();

            Rename.Column(OldName).OnTable(UserLogItem.TableName).To(NewName);

            IfDatabase(DatabaseProviders.SqlServerCE)
                .Delete.Column(OldName).FromTable(UserLogItem.TableName);
        }
    }
}
