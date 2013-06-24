using System.Collections.Generic;
using System.ServiceModel;
using Splash.SlaveWorker.Data;

namespace Splash.SlaveWorker.Interfaces
{
    [ServiceContract]
    public interface IWorker : ITaskDeleter
    {
        [OperationContract]
        List<MethodDescription> GetAllMethods();

        [OperationContract]
        OperationStatus Calculate(string methodName, List<KeyValuePair<string, double[][]>> inputParameters);

        [OperationContract]
        CalculationResult GetResult(string id);
    }
}