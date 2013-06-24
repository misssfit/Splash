using System;
using System.Collections.Generic;
using Splash.Common;
using Splash.McdaMethodsContract;
using Splash.SlaveWorker.Data;

namespace Splash.SlaveWorker
{
    public class CalculationTask : ActiveObject
    {
        private readonly List<KeyValuePair<string, double[][]>> _inputParameters;
        private readonly string _methodName;

        public CalculationTask(string methodName, List<KeyValuePair<string, double[][]>> inputParameters)
        {
            _inputParameters = inputParameters;
            _methodName = methodName;
            Id = Guid.NewGuid().ToString();
            CreationTimestamp = DateTime.UtcNow;
            Status = TaskStatus.NotStarted;
        }

        public string Id { get; private set; }
        public DateTime CreationTimestamp { get; private set; }
        public DateTime CalculationStartTimestamp { get; private set; }
        public DateTime CalculationFinishTimestamp { get; private set; }

        public TaskStatus Status { get; private set; }
        public double[][] Data { get; private set; }

        public TaskInfo TaskInfo
        {
            get
            {
                return new TaskInfo
                    {
                        Id = Id,
                        MethodName = _methodName,
                        CalculationStartTime = CalculationStartTimestamp,
                        CreationTime = CreationTimestamp,
                        CalculationFinishTime = CalculationFinishTimestamp,
                        Status = Status
                    };
            }
        }

        protected override void PerformAction()
        {
            Status = TaskStatus.InProgress;
            CalculationStartTimestamp = DateTime.UtcNow;
            try
            {
                IMcdaMethod methodObject = MethodRegistry.Instance.GetMethodObject(_methodName);
                var result = methodObject.Calculate(_inputParameters);
                Data = result;
            }
            catch (Exception e)
            {
                Console.WriteLine("!!! Exception in " + Id + " | " + e.Message);
                Status = TaskStatus.Corrupted;
                IsActive = false;
                return;
            }
            Status = TaskStatus.Calculated;
            IsActive = false;
            CalculationFinishTimestamp = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return CreationTimestamp + " | Id: " + Id + " | MethodName: " + _methodName + " | Status: " + Status;
        }
    }
}