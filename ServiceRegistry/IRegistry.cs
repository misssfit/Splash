using System.ServiceModel;
using Splash.Common;

namespace Splash.ServiceRegistry
{
    [ServiceContract]
    public interface IRegistry
    {
        [OperationContract]
        ServerConnectionInfo AssignServer();

        [OperationContract]
        string AssignServerId();

        [OperationContract]
        bool AcknowlegdeRegistration(string serviceId, string uri);
    }
}