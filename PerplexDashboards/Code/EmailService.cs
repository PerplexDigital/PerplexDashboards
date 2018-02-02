using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Xml;
using System.Xml.Serialization;
using System.Net.Mail;
using Umbraco.Core;
using PerplexDashboards.Models.UserDashboard;
using Umbraco.Core.Models.Membership;

namespace PerplexDashboards.Code
{
    public static class EmailService
    {
        public static void SendLockedAccountEmail(int userId)
        {
            IUser lockedUser = ApplicationContext.Current?.Services?.UserService?.GetUserById(userId);
            TimeSpan timeSinceLocked = DateTime.Now - lockedUser.LastLockoutDate;
            // Umbraco fires the AccountLocked event every time, even if the account was already locked,
            // so we have to check if it happened just now and not a while ago.
            if (lockedUser != null && timeSinceLocked.TotalSeconds < 1)
            {
                UserDashboardSettings settings = UserDashboardSettings.Get;
                if(!string.IsNullOrEmpty(settings?.LockedEmailRecipientAddress))
                {
                    SendEmail(
                        settings.LockedEmailRecipientAddress, 
                        settings.GetLockedEmailSubject(lockedUser.Name), 
                        settings.GetLockedEmailBodyHtml(lockedUser.Name)
                    );
                }
            }
        }

        private static void SendEmail(string toAddress, string subject, string body)
        {
            try
            {
                using (SmtpClient client = new SmtpClient())
                using (MailMessage email = new MailMessage())
                {
                    email.To.Add(toAddress);
                    email.Subject = subject;
                    email.Body = body;
                    email.IsBodyHtml = true;
                    client.Send(email);
                }
            }
            catch { }            
        }
    }
}