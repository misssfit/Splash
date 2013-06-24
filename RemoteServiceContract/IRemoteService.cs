using System.ServiceModel;

namespace Splash.RemoteServiceContract
{
    [ServiceContract]
    public interface IRemoteService
    {
        [OperationContract]
        IMethodInvocationResult Invoke(IMessage message);
    }
}