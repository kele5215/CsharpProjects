using System.Configuration;

namespace ServerConfSection
{
    public class ServerConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("serverNm", IsRequired = true)]
        public string ServerNm
        {
            get { return (string)base["serverNm"]; }
            set { base["serverNm"] = value; }
        }

        [ConfigurationProperty("serverAddress", IsRequired = true)]
        public string ServerAddress
        {
            get { return (string)base["serverAddress"]; }
            set { base["serverAddress"] = value; }
        }

        [ConfigurationProperty("user", IsRequired = true)]
        public string User
        {
            get { return (string)base["user"]; }
            set { base["user"] = value; }
        }

        [ConfigurationProperty("password", IsRequired = true)]
        public string Password
        {
            get { return (string)base["password"]; }
            set { base["password"] = value; }
        }

        [ConfigurationProperty("serviceNm", IsRequired = true)]
        public string ServiceNm
        {
            get { return (string)base["serviceNm"]; }
            set { base["serviceNm"] = value; }
        }

        [ConfigurationProperty("serviceInterval", IsRequired = true)]
        public string ServiceInterval
        {
            get { return (string)base["serviceInterval"]; }
            set { base["serviceInterval"] = value; }
        }

        public string ServiceErrNm { get; set; }

    }
}
