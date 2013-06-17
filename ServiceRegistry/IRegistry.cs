using System.ServiceModel;
using Common;

namespace ServiceRegistry
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