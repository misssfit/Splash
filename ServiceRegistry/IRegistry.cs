using System.Collections.Generic;
using System.ServiceModel;
using Splash.Common;

namespace Splash.ServiceRegistry
{
    [ServiceContract]
    public interface IRegistry
    {
        [OperationContract]
        StringRequestStatus AssignServer();

        [OperationContract]
        string AssignServerId();

        [OperationContract]
        bool AcknowlegdeRegistration(string serviceId, string uri);

        [OperationContract]
        List<string> SynchronizeInactiveWorkers();

    }
}