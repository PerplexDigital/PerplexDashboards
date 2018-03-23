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
using PerplexDashboards.Models.MemberDashboard.AccessLog;

namespace PerplexDashboards.Code
{
    public class PerplexDashboardsEventHandler : ApplicationEventHandler
    {
        protected override void ApplicationStarting(UmbracoApplicationBase umbApp, ApplicationContext appCtx)
        {
            ConfigureDashboards();
            RegisterUserEvents(appCtx.DatabaseContext);
            CreateDatabaseTablesIfNeeded(appCtx.DatabaseContext, appCtx.ProfilingLogger.Logger);            
            UserDashboardSettings.CreateIfNotExists();

            base.ApplicationStarting(umbApp, appCtx);
        }

        protected override void ApplicationStarted(UmbracoApplicationBase umbApp, ApplicationContext appCtx)
        {
            RunDatabaseMigrations(appCtx);
            base.ApplicationStarted(umbApp, appCtx);
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
            DatabaseHelper.CreateDatabaseTableIfNeeded<MemberAccessLogItem>(dbCtx, MemberAccessLogItem.TableName, logger);
        }
       
        private void RunDatabaseMigrations(ApplicationContext appCtx)
        {
            UserLogItem.RunMigrations(appCtx);
        }

        private void UsersMembershipProviderEventHandler(EventArgs e, DatabaseContext dbCtx)
        {
            if (e is IdentityAuditEventArgs eventArgs)
            {
                LogEvent(eventArgs, dbCtx);

                if (eventArgs.Action == AuditEvent.AccountLocked)
                {
                    EmailService.SendLockedAccountEmail(eventArgs.AffectedUser);
                }
            }
        }     

        private void LogEvent(IdentityAuditEventArgs eventArgs, DatabaseContext dbCtx)
        {
            // LoginFailed always has the AffectedUser as 0 when their is no user for the login,
            // see https://github.com/umbraco/Umbraco-CMS/blob/dev-v7/src/Umbraco.Core/Security/BackOfficeUserManager.cs#L609.
            // This is a bug in Umbraco's implementation, so we will ignore the AffectedUser in that case.
            int affectedUser = eventArgs.Action == AuditEvent.LoginFailed && eventArgs.Username != null && eventArgs.Comment != null
                ? -1
                : eventArgs.AffectedUser;

            var userLogItem = new UserLogItem(
                eventArgs.PerformingUser, affectedUser, eventArgs.Username, 
                eventArgs.Action, eventArgs.IpAddress, eventArgs.DateTimeUtc);

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