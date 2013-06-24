using System.Xml.Serialization;

namespace Splash.Common
{
    public class Subscription
    {
        [XmlAttribute]
        public string Id { get; set; }

        public string Host { get; set; }
        public string Port { get; set; }
    }
}