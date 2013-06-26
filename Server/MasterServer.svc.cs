using Splash.RemoteServiceContract;

namespace Splash.Server
{
    public class MasterServer : IMasterServer, IRemoteService
    {
        static MasterServer()
        {
            
        }

        public OperationStatus Invoke(RemoteMessage message)
        {
            using (var registry = new RegistryServiceReference.RegistryClient())
            {
                var serverUri = registry.AssignServer();
                using (var worker = new SlaveClient(serverUri))
                {
                    var slaveResponse = worker.Invoke(message);
                    DelegatedTaskStorage.Instance.Add(slaveResponse.Id, serverUri);
                    return slaveResponse;
                }
            }
          
            //use registry, get slave, delegate calculation, store data
        }
    }
}