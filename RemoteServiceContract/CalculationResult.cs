using System.Xml.Linq;

namespace Splash.RemoteServiceContract
{
    public class CalculationResult
    {
        public XElement Data { get; set; }
        public TaskStatus Status { get; set; }
        public TaskInfo Info { get; set; }
    }
}