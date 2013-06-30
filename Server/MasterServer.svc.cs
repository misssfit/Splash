using System;
using Splash.RemoteServiceContract;
using Splash.Server.RegistryServiceReference;

namespace Splash.Server
{
    public class MasterServer : IMasterServer, IRemoteService
    {
        static MasterServer()
        {

        }

        public OperationStatus Invoke(RemoteMessage message)
        {
            try
            {
                using (var registry = new RegistryClient())
                {
                    var serverUri = registry.AssignServer();
                    using (var worker = new SlaveClient(serverUri))
                    {
                        var slaveResponse = worker.Invoke(message);
                        DelegatedTaskStorage.Instance.Add(slaveResponse.Id, serverUri);
                        return slaveResponse;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new OperationStatus { Status = RequestStatus.Error };
            }

        }

        public CalculationResult GetResult(string id)
        {
            var serverUri = DelegatedTaskStorage.Instance.GetAssignedWorker(id);
            if (string.IsNullOrWhiteSpace(serverUri) == true)
            {
                return new CalculationResult { Status = TaskStatus.NotFound };
            }
            using (var worker = new SlaveClient(serverUri))
            {
                var slaveResponse = worker.GetResult(id);
                if (slaveResponse.Status == TaskStatus.Calculated || slaveResponse.Status == TaskStatus.Corrupted 
                    || slaveResponse.Status == TaskStatus.NotFound)
                {
                    DelegatedTaskStorage.Instance.RemoveAssignedWorker(id);
                }
                return slaveResponse;
            }
        }
    }
}