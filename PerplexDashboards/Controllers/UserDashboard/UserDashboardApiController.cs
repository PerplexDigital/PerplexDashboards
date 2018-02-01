using PerplexDashboards.Models.UserDashboard;
using System.Web.Http;
using System.Web.Security;
using Umbraco.Core;
using Umbraco.Web.Security.Providers;
using Umbraco.Web.WebApi;

namespace PerplexDashboards.Controllers.UserDashboard
{
    public class UserDashboardApiController : UmbracoAuthorizedApiController
    {
        private DatabaseContext DbContext => ApplicationContext.DatabaseContext;

        [HttpGet]
        public UserDashboardViewModel GetViewModel()
        {
            return new UserDashboardViewModel(Services.UserService, DbContext);
        }        

        [HttpPost]
        public SearchResults<ApiUserLogItem> Search(Filters filters)
        {          
            return ApiUserLogItem.Search(filters, DbContext);
        }

        [HttpGet]
        public UserPasswordPolicy GetPasswordPolicy()
        {
            UsersMembershipProvider userMembershipProvider = Membership.Providers["UsersMembershipProvider"] as UsersMembershipProvider;

            if (userMembershipProvider == null)
            {
                return null;
            }

            return new UserPasswordPolicy(userMembershipProvider);            
        }
    }
}
