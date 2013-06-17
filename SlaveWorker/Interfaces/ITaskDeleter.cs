using System.ServiceModel;
using CalculatingEngine.Data;

namespace SlaveWorker.Interfaces
{
    [ServiceContract]
    public interface ITaskDeleter
    {
        [OperationContract]
        OperationStatus DeleteTask(string id);
    }
}