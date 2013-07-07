using System;
using System.ServiceModel;
using Splash.Common.Logging;
using Splash.RemoteServiceContract;
using Splash.Server.RegistryServiceReference;

namespace Splash.Server
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class MasterServer : IMasterServer, IRemoteService
    {

        public OperationStatus Invoke(RemoteMessage message)
        {
            try
            {
                using (var registry = new RegistryClient())
                {
                    var reply = registry.AssignServer();
                    if (reply.Status == RequestStatus.Ok)
                    {
                        using (var worker = new SlaveClient(reply.Data))
                        {
                            var slaveResponse = worker.Invoke(message);
                            DelegatedTaskStorage.Instance.Add(slaveResponse.Id, reply.Data);
                            Logger.Instance.LogInfo(string.Format("Delegated task {0} to {1}", slaveResponse.Id, reply.Data));
                            return slaveResponse;
                        }
                    }
                    else
                    {
                        //??
                        //to do add to queue and wait
                        //for now return error
                        return new OperationStatus {Status = RequestStatus.Error};
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
                Logger.Instance.LogError(string.Format("On GetResult, task {0} not found", id));

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

        public OperationStatus DeleteTask(string id)
        {
            var serverUri = DelegatedTaskStorage.Instance.GetAssignedWorker(id);
            if (string.IsNullOrWhiteSpace(serverUri) == true)
            {
                Logger.Instance.LogError(string.Format("On DeleteTask, task {0} not found", id));

                return new OperationStatus { Id = id, Status = RequestStatus.Error };
            }
            using (var worker = new SlaveClient(serverUri))
            {
                var slaveResponse = worker.DeleteTask(id);
                DelegatedTaskStorage.Instance.RemoveAssignedWorker(id);
                return slaveResponse;
            }
        }
    }
}