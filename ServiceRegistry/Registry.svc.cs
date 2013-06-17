using System;
using System.ServiceModel;
using Common;

namespace ServiceRegistry
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Registry : Singleton<Registry>, IRegistry
    {
        private readonly SlaveController _slaveController = new SlaveController();


        public Registry()
        {
            _slaveController.Run();
        }

        public ServerConnectionInfo AssignServer()
        {
            throw new NotImplementedException();
        }

        public string AssignServerId()
        {
            string id = SlaveRegistry.Instance.ReserveId();
            return id;
        }


        public bool AcknowlegdeRegistration(string serviceId, string uri)
        {
            SlaveRegistry.Instance.AssignUriToServer(serviceId, uri);

            return true;
        }
    }
}