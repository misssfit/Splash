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
    }
}