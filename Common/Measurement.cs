using System.Xml.Serialization;

namespace Splash.Common
{
    public class Measurement
    {
        [XmlAttribute]
        public string ResourceId { get; set; }

        [XmlAttribute]
        public string Metric { get; set; }

        public string Timestamp { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return string.Format("{0}::{1}::{2}::{3}", ResourceId, Metric, Value, Timestamp);
        }
    }
}