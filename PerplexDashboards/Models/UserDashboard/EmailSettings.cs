using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PerplexDashboards.Models.UserDashboard
{
    public class EmailSettings
    {
        private UserDashboardSettings Config { get; }

        public string LockedEmailSubject { get; set; }
        public string LockedEmailBody { get; set; }
        public string LockedEmailRecipientAddress { get; set; }
        
        public EmailSettings()
        {
            Config = UserDashboardSettings.Get;            
        }

        public EmailSettings(UserDashboardSettings config)
        {
            Config = config;
            LockedEmailSubject = config.LockedEmailSubject;
            LockedEmailBody = config.LockedEmailBody;
            LockedEmailRecipientAddress = config.LockedEmailRecipientAddress;
        }

        public void Save()
        {
            Config.LockedEmailSubject = LockedEmailSubject;
            Config.LockedEmailBody = LockedEmailBody;
            Config.LockedEmailRecipientAddress = LockedEmailRecipientAddress;
            Config.Save();
        }
    }
}