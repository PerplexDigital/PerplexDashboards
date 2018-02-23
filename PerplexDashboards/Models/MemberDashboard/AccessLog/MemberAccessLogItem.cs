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
    [TableName(TableName)]
    [PrimaryKey(nameof(Id), autoIncrement = true)]
    public class MemberAccessLogItem
    {
        public const string TableName = "perplexUmbracoMemberAccessLog";

        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        public Guid MemberId { get; set; }
        public int UserId { get; set; }
        public DateTime Timestamp { get; set; }
        public string IpAddress { get; set; }

        public string Action
        {
            get => AccessAction?.ToString();
            set => AccessAction = Enum.TryParse(value, out MemberAccessAction accessAction)
                ? accessAction
                : (MemberAccessAction?)null;
        }

        [Ignore]
        public MemberAccessAction? AccessAction { get; set; }

        public static void Log(Guid memberId, int userId, MemberAccessAction action, string ipAddress = null, DatabaseContext databaseContext = null)
        {

            MemberAccessLogItem item = new MemberAccessLogItem
            {
                MemberId = memberId,
                UserId = userId,
                Timestamp = DateTime.UtcNow,
                IpAddress = ipAddress ?? HttpContext.Current?.Request?.UserHostAddress,
                AccessAction = action,
            };

            DatabaseContext dbCtx = databaseContext ?? ApplicationContext.Current.DatabaseContext;
            dbCtx.Database.Insert(item);
        }

        public static IList<MemberAccessLogItem> GetAll(DatabaseContext databaseContext = null, MemberAccessLogFilters filters = null)
        {
            DatabaseContext dbCtx = databaseContext ?? ApplicationContext.Current.DatabaseContext;

            Sql sql = new Sql()
                .Select("*")
                .From<MemberAccessLogItem>(dbCtx.SqlSyntax);

            if (filters != null)
            {
                Action<Expression<Func<MemberAccessLogItem, bool>>> addWhere =
                    expr => sql = sql.Where(expr, dbCtx.SqlSyntax);

                if (filters.From != null)
                {
                    addWhere(i => i.Timestamp >= filters.From.Value.Date);
                }

                if (filters.To != null)
                {
                    DateTime to = filters.To.Value.Date.AddDays(1);
                    addWhere(i => i.Timestamp < to);
                }

                if (filters.Action != null)
                {
                    string action = filters.Action.ToString();
                    addWhere(i => action == i.Action);
                }

                if (filters.MemberId != null)
                {
                    addWhere(i => i.MemberId == filters.MemberId);
                }

                if(filters.UserId != null)
                {
                    addWhere(i => i.UserId == filters.UserId);
                }

                if (!string.IsNullOrEmpty(filters.IpAddress))
                {
                    sql = sql.Where($"{nameof(IpAddress)} LIKE @0", new[] { $"%{filters.IpAddress}%" });
                }
            }

            sql = sql.OrderByDescending<MemberAccessLogItem>(item => item.Timestamp, dbCtx.SqlSyntax);

            return dbCtx.Database.Fetch<MemberAccessLogItem>(sql);
        }
    }
}
