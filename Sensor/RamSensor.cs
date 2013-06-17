using System.Diagnostics;
using Common;

namespace MeasurementSensor
{
    public class RamSensor : Sensor
    {
        private readonly PerformanceCounter _ramCounter;

        public RamSensor(int resolution)
            : base(resolution, "")
        {
            _ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        }

        protected override Measurement Measure()
        {
            var measurement = new Measurement
                {
                    Metric = "Free Ram",
                    Value = string.Format("{0} MB", _ramCounter.NextValue()),
                    ResourceId = _hostName
                };

            return measurement;
        }
    }
}