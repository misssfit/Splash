using System;
using System.Collections.Generic;
using System.ServiceModel;
using CalculatingEngine.Data;
using SlaveWorker.Data;
using SlaveWorker.Interfaces;

namespace SlaveWorker
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Worker : IWorker, IAdministrationService
    {
        public Worker()
        {
            MethodRegistry.Instance.Run();
        }

        public List<KeyValuePair<TaskPool, List<TaskInfo>>> GetAllTasks()
        {
            List<KeyValuePair<TaskPool, List<TaskInfo>>> result = TaskQueueManager.Instance.GetAllTasks();
            return result;
        }

        public OperationStatus PrioritizeTask(string id)
        {
            var operationStatus = new OperationStatus();
            operationStatus.Id = id;
            operationStatus.Status = TaskQueueManager.Instance.PrioritizeTask(id)
                                         ? RequestStatus.Ok
                                         : RequestStatus.Error;
            return operationStatus;
        }

        public OperationStatus DeleteAll(TaskPool pool)
        {
            var operationStatus = new OperationStatus();
            operationStatus.Status = TaskQueueManager.Instance.DeleteAll(pool) ? RequestStatus.Ok : RequestStatus.Error;
            return operationStatus;
        }

        public void RefreshMethodRegistry()
        {
            MethodRegistry.Instance.RefreshMethodRegistry();
        }

        public void ConfigureTasksCalculationTimeout(int timeoutValue)
        {
            if (timeoutValue > 0)
            {
                Configuration.TasksCalculationTimeout = timeoutValue;
                Console.WriteLine("tasks calculation timeout set to: " + timeoutValue + " [ms]");
            }
        }

        public void ConfigureCalculatedTasksTimeout(int timeoutValue)
        {
            if (timeoutValue > 0)
            {
                Configuration.CalculatedTasksTimeout = timeoutValue;
                Console.WriteLine("calculated tasks timeout set to: " + timeoutValue + " [ms]");
            }
        }

        public void ConfigureActiveTasksCount(int activeTasksCount)
        {
            if (activeTasksCount >= 0)
            {
                Configuration.ActiveTasksCount = activeTasksCount;
                Console.WriteLine("Number of active tasks is now set to: " + activeTasksCount);
            }
        }

        public List<MethodDescription> GetAllMethods()
        {
            return MethodRegistry.Instance.GetAll();
        }

        public OperationStatus Calculate(string methodName, List<KeyValuePair<string, double[][]>> inputParameters)
        {
            var operationStatus = new OperationStatus();
            operationStatus.Id = TaskQueueManager.Instance.CreateNewTask(methodName, inputParameters);
            operationStatus.Status = RequestStatus.NotReady;
            return operationStatus;
        }

        public OperationStatus DeleteTask(string id)
        {
            var operationStatus = new OperationStatus();
            operationStatus.Id = id;
            operationStatus.Status = TaskQueueManager.Instance.DeleteTask(id) ? RequestStatus.Ok : RequestStatus.Error;
            return operationStatus;
        }

        public CalculationResult GetResult(string id)
        {
            CalculationResult calculationResult = TaskQueueManager.Instance.GetResult(id);
            return calculationResult;
        }
    }
}