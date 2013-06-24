using System.Collections.Generic;
using System.ServiceModel;
using Splash.RemoteServiceContract;
using Splash.SlaveWorker.Data;

namespace Splash.SlaveWorker.Interfaces
{
    [ServiceContract]
    public interface IWorker : ITaskDeleter
    {
        [OperationContract]
        List<MethodDescription> GetAllMethods();


        [OperationContract]
        CalculationResult GetResult(string id);
    }
}