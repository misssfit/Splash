using System;
using System.Diagnostics;
using Splash.Common;

namespace Splash.MeasurementSensor
{
    public class CpuSensor : Sensor
    {
        private readonly PerformanceCounter _cpuCounter;


        public CpuSensor(int resolution)
            : base(resolution, "")
        {
            _cpuCounter = new PerformanceCounter
                {
                    CategoryName = "Processor",
                    CounterName = "% Processor Time",
                    InstanceName = "_Total"
                };
        }

        protected override Measurement Measure()
        {
            var measurement = new Measurement
                {
                    Metric = "Cpu",
                    Value = string.Format("{0}%", _cpuCounter.NextValue()),
                    ResourceId = _hostName,
                    Timestamp = DateTime.Now.ToString()
                };

            return measurement;
        }
    }
}