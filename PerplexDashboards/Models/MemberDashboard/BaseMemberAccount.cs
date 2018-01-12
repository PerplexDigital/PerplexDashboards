using System.Collections.Generic;
using System.Data;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace PerplexDashboards.Models.MemberDashboard
{
    public abstract class BaseMemberAccount<T> where T : BaseMemberAccount<T>
    {
        public int MemberId { get; set; }

        private IMember _member;
        public IMember Member => _member ?? (_member = memberService.GetById(MemberId));

        public string Name => Member.Name;
        public string Email => Member.Email;
        public string Icon => Member.ContentType.Icon;

        protected static IMemberService memberService = ApplicationContext.Current.Services.MemberService;
        protected static IEnumerable<T> GetAll(DatabaseContext db, string query)
        {
            if (memberService == null) return Enumerable.Empty<T>();
            return db.Database.Query<T>(query);
        }
    }
}