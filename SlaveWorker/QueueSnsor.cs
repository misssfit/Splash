using System;
using Common;
using MeasurementSensor;

namespace SlaveWorker
{
    public class QueueSnsor : Sensor
    {
        public QueueSnsor(int resolution, string hostName)
            : base(resolution, hostName)
        {
        }

        protected override Measurement Measure()
        {
            var measurement = new Measurement
                {
                    Metric = "Queue size",
                    Value = TaskQueueManager.Instance.GetQueueSize(),
                    ResourceId = _hostName,
                    Timestamp = DateTime.Now.ToString()
                };

            return measurement;
        }
    }
}