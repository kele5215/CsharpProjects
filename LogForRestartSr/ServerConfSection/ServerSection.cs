using System.Configuration;

namespace ServerConfSection
{
    /// <summary>
    /// Summary description for ServerSection
    /// </summary>
    public class ServerSection : ConfigurationSection
    {
        public ServerSection()
        {
        }
        [ConfigurationProperty("serverelements", IsRequired = true)]
        [ConfigurationCollection(typeof(ServerGroup), AddItemName = "addServer")]
        public ServerGroup ServerElements
        {
            get { return (ServerGroup)base["serverelements"]; }
            set { base["serverelements"] = value; }
        }
    }
}
