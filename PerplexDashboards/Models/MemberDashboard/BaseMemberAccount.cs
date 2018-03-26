using System;
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
        public IMember Member { get; }

        public string Name => Member.Name;
        public string Email => Member.Email;
        public string Icon => Member.ContentType.Icon;
       
        public BaseMemberAccount(IMember member)
        {
            Member = member;
        }

        protected static IList<T> GetAll(IMemberService memberService, Func<IMember, bool> filterFn, Func<IMember, T> mapFn)
        {
            return memberService.GetAll(0, int.MaxValue, out int _)
                .Where(filterFn)
                .Select(mapFn)
                .ToList();
        }
    }
}