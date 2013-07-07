using System;
using System.Collections.Generic;
using System.ServiceModel;
using Splash.Common;
using Splash.Common.Logging;
using Splash.RemoteServiceContract;

namespace Splash.ServiceRegistry
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Registry : Singleton<Registry>, IRegistry
    {
        private readonly SlaveController _slaveController = new SlaveController();


        public Registry()
        {
            _slaveController.Run();
        }

        public StringRequestStatus AssignServer()
        {
            var worker = SlaveRegistry.Instance.AssignServer();
            var result = new StringRequestStatus();
            if (string.IsNullOrWhiteSpace(worker) == true)
            {
                result.Status = RequestStatus.Error;
            }
            else
            {
                result.Status = RequestStatus.Ok;
                result.Data = worker;
                Logger.Instance.LogInfo(string.Format("Assigned worker: {0}", worker));
                
            }
            return result;
        }

        public string AssignServerId()
        {
            string id = SlaveRegistry.Instance.ReserveId();
            Logger.Instance.LogInfo(string.Format("Reserved Id: {0}", id));

            return id;
        }


        public bool AcknowlegdeRegistration(string serviceId, string uri)
        {
            var result = SlaveRegistry.Instance.AssignUriToServer(serviceId, uri);
            Logger.Instance.LogInfo(string.Format("Acknowlegded Registration for: {0} at {1}", serviceId, uri));

            return result;
        }

        public List<string> SynchronizeInactiveWorkers()
        {
            var inactiveServers = SlaveRegistry.Instance.GetInactiveWorkers();
            return inactiveServers;
        }
    }
}