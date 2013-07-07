using System.ServiceModel;
using Splash.RemoteServiceContract;

namespace Splash.Server
{
    public class SlaveClient : ClientBase<IRemoteService>, IRemoteService
    {
        public SlaveClient(string remoteAddress)
            : base(new WSHttpBinding(), new EndpointAddress(remoteAddress))
        {
            
        }
        public OperationStatus Invoke(RemoteMessage message)
        {
            return Channel.Invoke(message);
        }

        public CalculationResult GetResult(string id)
        {
            return Channel.GetResult(id);

        }

        public OperationStatus DeleteTask(string id)
        {
            return Channel.DeleteTask(id);
        }
    }
}