using PerplexDashboards.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace PerplexDashboards.Models.MemberDashboard
{
    [TableName(TableName)]
    [PrimaryKey(nameof(Id), autoIncrement = true)]
    public class MemberLogItem
    {
        public const string TableName = "perplexUmbracoMemberLog";

        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public string IP { get; set; }

        public string AuditAction
        {
            get => Action?.ToString();
            set 
            {                
                if(Enum.TryParse(value, out MemberAuditAction auditAction))
                {
                    Action = auditAction;
                }
            }
        }

        [Ignore]
        public MemberAuditAction? Action { get; set; }

        public string Url { get; set; }
       
        private Lazy<string> _email;
        [Ignore]
        public string Email => _email.Value;
      
        public MemberLogItem()
        {
            _email = new Lazy<string>(() => ApplicationContext.Current.Services.MemberService.GetById(UserId)?.Email);
        }

        public static void Log(int userId, MemberAuditAction action, string ip = null, string url = null, DatabaseContext databaseContext = null)
        {
            HttpRequest request = HttpContext.Current?.Request;
            if (request != null)
            {
                MemberLogItem item = new MemberLogItem
                {
                    Date = DateTime.UtcNow,
                    IP = ip ?? request?.UserHostAddress,
                    Url = url ?? request?.RawUrl,
                    Action = action,
                    UserId = userId
                };

                DatabaseContext dbCtx = databaseContext ?? ApplicationContext.Current.DatabaseContext;
                dbCtx.Database.Insert(item);
            }
        }

        /// <summary>
        /// Levert de laatste locked out datetime op voor de gegeven member
        /// </summary>
        /// <param name="memberId">E-mail van user</param>
        /// <returns></returns>
        public static DateTime? GetLastLockedOutTime(int memberId, DatabaseContext databaseContext = null)
        {
            DatabaseContext dbCtx = databaseContext ?? ApplicationContext.Current.DatabaseContext;
            MemberLogItem logItem = dbCtx.Database.FirstOrDefault<MemberLogItem>(
                new Sql()
                .Select("*")                
                .From<MemberLogItem>(dbCtx.SqlSyntax)                
                .Where<MemberLogItem>(item => item.UserId == memberId && item.AuditAction == MemberAuditAction.LockedOut.ToString(), dbCtx.SqlSyntax)
                .OrderByDescending<MemberLogItem>(item => item.Date, dbCtx.SqlSyntax));

            return logItem?.Date;
        }

        public static IList<MemberLogItem> GetAll(DatabaseContext databaseContext = null, MemberFilters filters = null)
        {
            DatabaseContext dbCtx = databaseContext ?? ApplicationContext.Current.DatabaseContext;

            Sql sql = new Sql()
                .Select("*")
                .From<MemberLogItem>(dbCtx.SqlSyntax);

            if (filters != null)
            {
                Action<Expression<Func<MemberLogItem, bool>>> addWhere =
                    expr => sql = sql.Where(expr, dbCtx.SqlSyntax);

                if (filters.From != null)
                {
                    addWhere(i => i.Date >= filters.From.Value.Date);
                }

                if (filters.To != null)
                {
                    DateTime to = filters.To.Value.Date.AddDays(1);
                    addWhere(i => i.Date < to);
                }

                if (filters.Action != null)
                {
                    string action = filters.Action.ToString();
                    addWhere(i => action == i.AuditAction);
                }

                if (filters.MemberId != null)
                {
                    addWhere(i => i.UserId == filters.MemberId);
                }

                if (!string.IsNullOrEmpty(filters.IpAddress))
                {
                    // TODO: Broken
                    // sql = sql.Where($"{nameof(IP)} LIKE '%' + @0 + '%'", new[] { filters.IpAddress });
                }
            }

            sql = sql.OrderByDescending<MemberLogItem>(item => item.Date, dbCtx.SqlSyntax);

            return dbCtx.Database.Fetch<MemberLogItem>(sql);
        }

        public string GetDescription(MemberAuditAction? action)
        {
            return action?.ToString();

            // Possible => translate?
            switch(action)
            {
                case MemberAuditAction.Activated:
                    break;
                case MemberAuditAction.ActivatedByAdministrator:
                    break;
                case MemberAuditAction.FailedLogin:
                    break;
                case MemberAuditAction.ForgotPasswordFinished:
                    break;
                case MemberAuditAction.ForgotPasswordRequested:
                    break;
                case MemberAuditAction.LockedOut:
                    break;
                case MemberAuditAction.Login:
                    break;
                case MemberAuditAction.Logout:
                    break;
                case MemberAuditAction.Register:
                    break;
                case MemberAuditAction.UpdateAccount:
                    break;
                case MemberAuditAction.UpdatePassword:
                    break;
            }
        }
    }   
}
