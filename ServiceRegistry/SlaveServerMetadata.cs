namespace Splash.ServiceRegistry
{
    public class SlaveServerMetadata
    {
        public string Id { get; set; }
        public string Uri { get; set; }
        public int QueueSize { get; set; }
        public int PerformanceMetric { get { return QueueSize; }
        }


        public WorkerStatus Status { get; set; }
    }
}