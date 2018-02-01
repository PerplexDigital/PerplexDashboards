using System;
using Umbraco.Core;
using Umbraco.Core.Auditing;
using Umbraco.Core.Security;
using Umbraco.Core.Logging;
using System.Web.Hosting;
using System.Xml.Linq;
using System.IO;
using System.Xml.XPath;
using PerplexDashboards.Models.UserDashboard;
using PerplexDashboards.Models.MemberDashboard;
using Umbraco.Web.Security.Providers;
using System.Web.Security;
using System.Diagnostics;

namespace PerplexDashboards.Code
{
    public class PerplexDashboardsEventHandler : ApplicationEventHandler
    {
        protected override void ApplicationStarting(UmbracoApplicationBase umbApp, ApplicationContext appCtx)
        {
            ConfigureDashboards();
            RegisterUserEvents(appCtx.DatabaseContext);
            CreateDatabaseTablesIfNeeded(appCtx.DatabaseContext, appCtx.ProfilingLogger.Logger);

            base.ApplicationStarting(umbApp, appCtx);
        }        

        private delegate void HandleUserEvent(object o, EventArgs e);

        private void RegisterUserEvents(DatabaseContext dbCtx)
        {
            // Inject DatabaseContext into event handler
            EventHandler userEventHandler = (o, e) =>
                UsersMembershipProviderEventHandler(e, dbCtx);
            
            BackOfficeUserManager.AccountLocked += userEventHandler;
            BackOfficeUserManager.AccountUnlocked += userEventHandler;
            BackOfficeUserManager.ForgotPasswordRequested += userEventHandler;
            BackOfficeUserManager.ForgotPasswordChangedSuccess += userEventHandler;
            BackOfficeUserManager.LoginFailed += userEventHandler;
            BackOfficeUserManager.LoginRequiresVerification += userEventHandler;
            BackOfficeUserManager.LoginSuccess += userEventHandler;
            BackOfficeUserManager.LogoutSuccess += userEventHandler;
            BackOfficeUserManager.PasswordChanged += userEventHandler;
            BackOfficeUserManager.PasswordReset += userEventHandler;
            BackOfficeUserManager.ResetAccessFailedCount += userEventHandler;
        }

        private void CreateDatabaseTablesIfNeeded(DatabaseContext dbCtx, ILogger logger = null)
        {
            DatabaseHelper.CreateDatabaseTableIfNeeded<UserLogItem>(dbCtx, UserLogItem.TableName, logger);
            DatabaseHelper.CreateDatabaseTableIfNeeded<MemberLogItem>(dbCtx, MemberLogItem.TableName, logger);            
        }
       
        private void UsersMembershipProviderEventHandler(EventArgs e, DatabaseContext dbCtx)
        {
            if (e is IdentityAuditEventArgs eventArgs)
            {
                LogEvent(eventArgs, dbCtx);
            }
        }
        
        private void LogEvent(IdentityAuditEventArgs eventArgs, DatabaseContext dbCtx)
        {     
            // AffectedUser appears to be 0 all the time, whether it actually affects user 0 or not,
            // clearly something is wrong and when it's supposed to be null (or -1, since this is not a nullable int)
            // it is 0 instead (== default(int)).
            // Unfortunately, this means we cannot use this value, thus we will just ignore it when it's 0            
            int affectedUser = eventArgs.AffectedUser > 0 ? eventArgs.AffectedUser : -1;
            var userLogItem = new UserLogItem(eventArgs.PerformingUser, affectedUser, eventArgs.Action, eventArgs.IpAddress, eventArgs.DateTimeUtc);

            userLogItem.Save(dbCtx);
        }      

        private void ConfigureDashboards()
        {
            string dashboardConfigPath = HostingEnvironment.MapPath("~/config/Dashboard.config");
            if(File.Exists(dashboardConfigPath))
            {
                XDocument document = XDocument.Parse(File.ReadAllText(dashboardConfigPath));

                bool memberDashboardWasCreated = CreateMemberDashboardIfNeeded(document);
                bool userDashboardWasCreated = CreateUserDashboardIfNeeded(document);

                if (memberDashboardWasCreated || userDashboardWasCreated)
                {
                    // Only write back to disk on change, 
                    // as this will trigger an app restart
                    document.Save(dashboardConfigPath);
                }
            }
        }

        private bool CreateMemberDashboardIfNeeded(XDocument document)
        {
            XElement memberDashboard = document.XPathSelectElement("/dashBoard/section[@alias='PerplexDashboards_MemberDashboard']");

            if (memberDashboard == null)
            {
                // Add dashboard as child of /dashBoard.
                string xmlFilePath = HostingEnvironment.MapPath(Path.Combine(Constants.PLUGIN_ROOT_DIRECTORY, "xml", "memberDashboard.xml"));
                AddXmlFromFileToElement(document, "/dashBoard", xmlFilePath);
                return true;
            }            

            return false;
        }

        private bool CreateUserDashboardIfNeeded(XDocument document)
        {
            XElement userDashboard = document.XPathSelectElement("/dashBoard/section[@alias='PerplexDashboards_UserDashboard']");            
            if (userDashboard == null)
            {
                // Add dashboard as child of /dashBoard.
                string xmlFilePath = HostingEnvironment.MapPath(Path.Combine(Constants.PLUGIN_ROOT_DIRECTORY, "xml", "userDashboard.xml"));
                AddXmlFromFileToElement(document, "/dashBoard", xmlFilePath);
                return true;
            }

            return false;
        }

        private XDocument AddXmlFromFileToElement(XDocument document, string elementXPath, string xmlFilePath)
        {
            XElement element = document.XPathSelectElement(elementXPath);
            if (element != null)
            {
                if (File.Exists(xmlFilePath))
                {
                    XElement xmlData = XElement.Parse(File.ReadAllText(xmlFilePath));
                    element.Add(xmlData);
                }
            }

            return document;
        }
    }
}