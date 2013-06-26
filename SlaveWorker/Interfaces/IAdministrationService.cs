﻿using System.Collections.Generic;
using System.ServiceModel;
using Splash.RemoteServiceContract;
using Splash.SlaveWorker.Data;

namespace Splash.SlaveWorker.Interfaces
{
    [ServiceContract]
    public interface IAdministrationService : ITaskDeleter
    {
        [OperationContract]
        List<KeyValuePair<TaskPool, List<TaskInfo>>> GetAllTasks();

        [OperationContract]
        OperationStatus PrioritizeTask(string id);

        [OperationContract]
        OperationStatus DeleteAll(TaskPool pool);

        [OperationContract]
        void RefreshMethodRegistry();

        [OperationContract]
        void ConfigureTasksCalculationTimeout(int timeoutValue);

        [OperationContract]
        void ConfigureCalculatedTasksTimeout(int timeoutValue);

        [OperationContract]
        void ConfigureActiveTasksCount(int activeTasksCount);
    }
}