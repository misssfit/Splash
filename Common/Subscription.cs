using System.Xml.Serialization;

namespace Common
{
    public class Subscription
    {
        [XmlAttribute]
        public string Id { get; set; }

        public string Host { get; set; }
        public string Port { get; set; }
    }
}