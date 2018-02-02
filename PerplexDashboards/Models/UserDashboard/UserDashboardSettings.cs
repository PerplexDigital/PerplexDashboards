using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Xml;
using System.Xml.Serialization;

namespace PerplexDashboards.Models.UserDashboard
{
    /// <summary>
    /// Configuration settings read from a config XML file
    /// </summary>
    [XmlRoot("settings")]
    public class UserDashboardSettings
    {
        private const string FILE_PATH = "~/Config/userDashboardSettings.config";

        /// <summary>
        /// Cached instance of configuration
        /// </summary>
        private static UserDashboardSettings Config;

        [XmlAttribute("version")]
        public int Version = 1;

        [XmlIgnore]
        public string EmailTemplate { get; set; }

        [XmlElement("emailTemplate")]
        public XmlCDataSection EmailTemplateCData
        {
            get => new XmlDocument().CreateCDataSection(EmailTemplate);
            set => EmailTemplate = value.Value;
        }

        [XmlElement("lockedEmailSubject")]
        public string LockedEmailSubject { get; set; }

        [XmlElement("lockedEmailRecipientAddress")]
        public string LockedEmailRecipientAddress { get; set; }

        [XmlIgnore]
        public string LockedEmailBody { get; set; }

        [XmlElement("lockedEmailBody")]
        public XmlCDataSection LockedEmailBodyCData
        {
            get => new XmlDocument().CreateCDataSection(LockedEmailBody);
            set => LockedEmailBody = value.Value;
        }

        public string GetLockedEmailSubject(string username)
        {
            return ReplaceTags(LockedEmailSubject, username);                            
        }

        public string GetLockedEmailBodyHtml(string username)
        {
            return ReplaceTags(EmailTemplate.Replace("[#body#]", LockedEmailBody), username);                
        }

        private string ReplaceTags(string input, string username)
        {
            return input                
                .Replace("[#username#]", username)
                .Replace("[#website#]", HttpContext.Current?.Request?.Url?.GetLeftPart(UriPartial.Authority))
                .Replace("[#datetime#]", $"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}");
        }

        private UserDashboardSettings() { }

        public static UserDashboardSettings Get
        {
            get
            {
                if (Config == null)
                {
                    // If the file is not there => Create with defaults
                    CreateIfNotExists();

                    // Create from configuration file
                    string path = GetFilePath();

                    XmlSerializer serializer = new XmlSerializer(typeof(UserDashboardSettings));

                    // Read from file
                    using (var reader = new StreamReader(GetFilePath()))
                    {
                        Config = (UserDashboardSettings)serializer.Deserialize(reader);
                    }
                }

                return Config;
            }
        }

        public static void CreateIfNotExists()
        {
            // Create from configuration file
            string path = GetFilePath();

            // If it is not there yet, serialize our defaults to file
            if (!File.Exists(path))
            {
                DefaultConfig.Save();

                XmlSerializer serializer = new XmlSerializer(typeof(UserDashboardSettings));

                using (StreamWriter file = new StreamWriter(path))
                {
                    serializer.Serialize(file, DefaultConfig);
                }
            }
        }

        public void Save()
        {
            // Create from configuration file
            string path = GetFilePath();

            XmlSerializer serializer = new XmlSerializer(typeof(UserDashboardSettings));

            using (StreamWriter file = new StreamWriter(path))
            {
                serializer.Serialize(file, this);
            }
        }

        private static string GetFilePath()
        {
            return HostingEnvironment.MapPath(FILE_PATH);
        }

        private static readonly UserDashboardSettings DefaultConfig = new UserDashboardSettings
        {
            EmailTemplate = $@"[#body#]",

            LockedEmailSubject = "An account was locked",

            LockedEmailRecipientAddress = "name@host.com"
        };
    }
}