using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Splash.RemoteServiceContract
{
    public class ObjectParameter 
    {
        public string Name { get; set; }
        public XElement Value { get; set; }
    }
}