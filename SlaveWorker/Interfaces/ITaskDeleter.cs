using System.ServiceModel;
using Splash.RemoteServiceContract;
using Splash.SlaveWorker.Data;

namespace Splash.SlaveWorker.Interfaces
{
    [ServiceContract]
    public interface ITaskDeleter
    {
        [OperationContract]
        IMethodInvocationResult DeleteTask(string id);
    }
}