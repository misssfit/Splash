using CalculatingEngine.Data;

namespace SlaveWorker.Data
{
    public class CalculationResult
    {
        public double[][] Data { get; set; }
        public TaskStatus Status { get; set; }
        public TaskInfo Info { get; set; }
    }
}