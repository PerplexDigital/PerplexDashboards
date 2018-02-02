using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Umbraco.Core.Models;
using Umbraco.Web.WebApi;
using Umbraco.Core;
using System.Net.Http;
using System.Globalization;
using System.Net;
using PerplexDashboards.Models.MemberDashboard;
using System.Web.Security;
using Umbraco.Web.Security.Providers;
using PerplexDashboards.Models.MemberDashboard.ActivityLog;
using PerplexDashboards.Models;

namespace PerplexDashboards.Controllers.MemberDashboard
{
    public class MemberDashboardApiController : UmbracoAuthorizedApiController
    {
        [HttpGet]
        public PagedResult<LockedMembersListViewResponse> GetLockedMembersListView([FromUri]ListViewRequest request)
        {
            int toSkip = (request.PageNumber - 1) * request.PageSize;
            var filterNormalized = string.IsNullOrEmpty(request.Filter) ? "" : request.Filter.ToLower();
            Func<DateTime, string> dateFormat = d => d.ToString("d MMMM yyyy", new CultureInfo("nl-NL"));

            Func<IEnumerable<LockedMemberAccount>, IOrderedEnumerable<LockedMemberAccount>> orderBy = allMembers =>
            {
                bool orderAscending = request.OrderDirection.ToLower() == "asc";

                switch (request.OrderBy)
                {
                    case nameof(LockedMemberAccount.Email):
                        return (orderAscending
                            ? allMembers.OrderBy(a => a.Email)
                            : allMembers.OrderByDescending(a => a.Email)
                        ).ThenBy(a => a.Name);

                    case nameof(LockedMemberAccount.LastLoginDate):
                        return (orderAscending
                            ? allMembers.OrderBy(a => a.LastLoginDate)
                            : allMembers.OrderByDescending(a => a.LastLoginDate)
                        ).ThenBy(a => a.Name);

                    default:
                        return orderAscending
                            ? allMembers.OrderBy(a => a.Name)
                            : allMembers.OrderByDescending(a => a.Name);
                }
            };

            List<LockedMemberAccount> allLockedMembers = orderBy(
                LockedMemberAccount.GetAll(ApplicationContext.DatabaseContext)
                .Where(member =>
                    string.IsNullOrEmpty(request.Filter) ||
                    member.Name.ToLower().Contains(filterNormalized) ||
                    member.Email.ToLower().Contains(filterNormalized) ||
                    dateFormat(member.LastLoginDate).ToLower().Contains(filterNormalized)
                )
            ).ToList();

            var pagedResult = new PagedResult<LockedMembersListViewResponse>(allLockedMembers.Count, request.PageNumber, request.PageSize);

            pagedResult.Items = allLockedMembers.Skip(toSkip).Take(request.PageSize).Select(a => new LockedMembersListViewResponse
            {
                id = a.MemberId,
                icon = a.Icon,
                name = a.Name,
                published = true,

                Email = a.Email,
                LastLoginDate = dateFormat(a.LastLoginDate)
            });

            return pagedResult;
        }

        [HttpGet]
        public HttpResponseMessage UnlockMember(int memberId)
        {
            IMember member = null;
            HttpResponseMessage errorResponse = null;

            if (!TryGetMember(memberId, out member, out errorResponse))
            {
                return errorResponse;
            }

            if (member.IsLockedOut)
            {
                member.SetValue(Constants.Conventions.Member.IsLockedOut, false);
                member.SetValue(Constants.Conventions.Member.FailedPasswordAttempts, 0);

                ApplicationContext.Services.MemberService.Save(member, raiseEvents: false);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        public PagedResult<UnapprovedMembersListViewResponse> GetUnapprovedMembersListView([FromUri]ListViewRequest request)
        {
            int toSkip = (request.PageNumber - 1) * request.PageSize;
            var filterNormalized = string.IsNullOrEmpty(request.Filter) ? "" : request.Filter.ToLower();
            Func<DateTime, string> dateFormat = d => d.ToString("d MMMM yyyy", new CultureInfo("nl-NL"));

            Func<IEnumerable<UnapprovedMemberAccount>, IOrderedEnumerable<UnapprovedMemberAccount>> orderBy = allMembers =>
            {
                bool orderAscending = request.OrderDirection.ToLower() == "asc";

                switch (request.OrderBy)
                {
                    case nameof(UnapprovedMemberAccount.Email):
                        return (orderAscending
                            ? allMembers.OrderBy(a => a.Email)
                            : allMembers.OrderByDescending(a => a.Email)
                        ).ThenBy(a => a.Name);

                    case nameof(UnapprovedMemberAccount.CreateDate):
                        return (orderAscending
                            ? allMembers.OrderBy(a => a.CreateDate)
                            : allMembers.OrderByDescending(a => a.CreateDate)
                        ).ThenBy(a => a.Name);

                    default:
                        return orderAscending
                            ? allMembers.OrderBy(a => a.Name)
                            : allMembers.OrderByDescending(a => a.Name);
                }
            };

            List<UnapprovedMemberAccount> allLockedMembers = orderBy(
                UnapprovedMemberAccount.GetAll(ApplicationContext.DatabaseContext)
                .Where(member =>
                    string.IsNullOrEmpty(request.Filter) ||
                    member.Name.ToLower().Contains(filterNormalized) ||
                    member.Email.ToLower().Contains(filterNormalized) ||
                    dateFormat(member.CreateDate).ToLower().Contains(filterNormalized)
                )
            ).ToList();

            var pagedResult = new PagedResult<UnapprovedMembersListViewResponse>(allLockedMembers.Count, request.PageNumber, request.PageSize);

            pagedResult.Items = allLockedMembers.Skip(toSkip).Take(request.PageSize).Select(a => new UnapprovedMembersListViewResponse
            {
                id = a.MemberId,
                icon = a.Icon,
                name = a.Name,
                published = true,

                Email = a.Email,
                CreateDate = dateFormat(a.CreateDate)
            });

            return pagedResult;
        }

        [HttpGet]
        public HttpResponseMessage ApproveMember(int memberId)
        {
            IMember member;
            HttpResponseMessage errorResponse;
            if (!TryGetMember(memberId, out member, out errorResponse))
            {
                return errorResponse;
            }

            if (!member.IsApproved)
            {
                member.SetValue(Constants.Conventions.Member.IsApproved, true);
                ApplicationContext.Services.MemberService.Save(member, raiseEvents: false);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        public HttpResponseMessage DeleteMember(int memberId)
        {
            IMember member;
            HttpResponseMessage errorResponse;
            if (!TryGetMember(memberId, out member, out errorResponse))
            {
                return errorResponse;
            }

            ApplicationContext.Services.MemberService.Delete(member);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        public PagedResult<MembersLogListViewResponse> GetMembersLogListView([FromUri]MembersLogListViewRequest request)
        {
            int toSkip = (request.PageNumber - 1) * request.PageSize;
            var filterNormalized = string.IsNullOrEmpty(request.Filter) ? "" : request.Filter.ToLower();
            Func<DateTime, string> dateFormat = d => d.ToString("d MMM yyyy HH:mm", new CultureInfo("nl-NL"));

            Func<IEnumerable<MemberLogItem>, IOrderedEnumerable<MemberLogItem>> orderBy = allMembers =>
            {
                bool orderAscending = request.OrderDirection.ToLower() == "asc";

                switch (request.OrderBy)
                {
                    case nameof(MembersLogListViewResponse.DateTime):
                        return (orderAscending
                            ? allMembers.OrderBy(a => a.Date)
                            : allMembers.OrderByDescending(a => a.Date)
                        ).ThenBy(a => a.Email);

                    case nameof(MembersLogListViewResponse.Action):
                        return (orderAscending
                            ? allMembers.OrderBy(a => a.Action)
                            : allMembers.OrderByDescending(a => a.Action)
                        ).ThenByDescending(a => a.Date);

                    case nameof(MembersLogListViewResponse.IPAddress):
                        return (orderAscending
                            ? allMembers.OrderBy(a => a.IP)
                            : allMembers.OrderByDescending(a => a.IP)
                        ).ThenByDescending(a => a.Date);

                    default:
                        return orderAscending
                            ? allMembers.OrderBy(a => a.Email)
                            : allMembers.OrderByDescending(a => a.Email);
                }
            };

            List<MemberLogItem> allMembersLog = orderBy(
                MemberLogItem.GetAll(ApplicationContext.DatabaseContext)
                .Where(member =>
                    (request.Id == null || request.Id == member.UserId) &&
                    (string.IsNullOrEmpty(request.Filter) || (
                        (member.Email != null && member.Email.ToLower().Contains(filterNormalized)) ||
                        (member.Action != null && member.Action.ToString().ToLower().Contains(filterNormalized)) ||
                        (member.IP != null && member.IP.ToLower().Contains(filterNormalized)) ||
                        (dateFormat(member.Date).ToLower().Contains(filterNormalized))
                    ))
                )
            ).ToList();

            var pagedResult = new PagedResult<MembersLogListViewResponse>(allMembersLog.Count, request.PageNumber, request.PageSize);

            pagedResult.Items = allMembersLog.Skip(toSkip).Take(request.PageSize).Select(a => new MembersLogListViewResponse
            {
                id = a.UserId,
                icon = "icon-footprints",
                name = a.Email,
                published = true,

                DateTime = dateFormat(a.Date),
                Action = a.GetDescription(a.Action),
                IPAddress = a.IP
            });

            return pagedResult;
        }

        /// <summary>
        /// Tries to retrieve the member with the specified Member ID.
        /// Returns false upon failure and sets the given errorResponse to an appropriate HttpResponseMessage (statuscode + message)
        /// </summary>
        /// <param name="memberId">ID of member to retrieve</param>
        /// <param name="member">Member property to set with the member, if found</param>
        /// <param name="errorResponse">HttpResponseMessage to set upon failure with proper statuscode + error message</param>
        /// <returns></returns>
        private bool TryGetMember(int memberId, out IMember member, out HttpResponseMessage errorResponse)
        {
            member = null;
            errorResponse = null;

            var memberService =  ApplicationContext.Services.MemberService;

            if (memberService == null)
            {
                errorResponse = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Could not obtain MemberService");
                return false;
            }

            member = memberService.GetById(memberId);

            if (member == null)
            {
                errorResponse = Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Member with id { memberId } not found");
                return false;
            }

            return true;
        }

        // NEW ->

        private DatabaseContext DbContext => ApplicationContext.DatabaseContext;

        [HttpGet]
        public MemberActivityLogViewModel GetActivityLogViewModel(Guid? memberGuid)
        {
            return new MemberActivityLogViewModel(Services.MemberService, DbContext, memberGuid);
        }

        [HttpPost]
        public SearchResults<ApiMemberLogItem> SearchActivityLog(MemberFilters filters)
        {
            return ApiMemberLogItem.Search(filters, DbContext);
        }

        [HttpGet]
        public MemberPasswordPolicy GetPasswordPolicy()
        {
            MembersMembershipProvider membersMembershipProvider = Membership.Providers["UmbracoMembershipProvider"] as MembersMembershipProvider;

            if (membersMembershipProvider == null)
            {
                return null;
            }

            return new MemberPasswordPolicy(membersMembershipProvider);            
        }
    }
}
