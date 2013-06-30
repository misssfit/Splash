using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Splash.Common;
using Splash.RemoteMethodsContract;
using Splash.RemoteServiceContract;
using Splash.SlaveWorker.Data;

namespace Splash.SlaveWorker
{
    public class RemoteTask : ActiveObject
    {


        private readonly string _methodName;
        private Lazy<IRemoteMethod> _remoteMethod;
        private List<ObjectParameter> _parameters;

        public RemoteTask(RemoteMessage taskInfo)
        {
            _parameters = taskInfo.MethodParameters;
            _methodName = taskInfo.MethodName;
            Id = Guid.NewGuid().ToString();
            CreationTimestamp = DateTime.UtcNow;
            Status = TaskStatus.NotStarted;
            _remoteMethod = MethodResolver.Instance.Resolve(_methodName);

        }

        public string Id { get; private set; }
        public DateTime CreationTimestamp { get; private set; }
        public DateTime CalculationStartTimestamp { get; private set; }
        public DateTime CalculationFinishTimestamp { get; private set; }

        public TaskStatus Status { get; private set; }
        public XElement Data { get; private set; }

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
                var result = _remoteMethod.Value.Calculate(_parameters);
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