﻿using PerplexDashboards.Models.UserDashboard;
using System.Web.Http;
using Umbraco.Core;
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
            return new UserPasswordPolicy
            {
                ForgotPasswordLinkAvailable = true,
                MaximumPasswordPage = 5,
                MinimumNonAlphaNumericCharacters = 7,
                PasswordHistory = 3
            };
        }
    }
}
