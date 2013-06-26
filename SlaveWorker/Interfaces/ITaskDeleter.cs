using System.ServiceModel;
using Splash.RemoteServiceContract;
using Splash.SlaveWorker.Data;

namespace Splash.SlaveWorker.Interfaces
{
    [ServiceContract]
    public interface ITaskDeleter
    {
        [OperationContract]
        OperationStatus DeleteTask(string id);
    }
}