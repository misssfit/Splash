using Splash.RemoteServiceContract;

namespace Splash.Server
{
    public class MasterServer : IMasterServer, ICalculatingService, IRemoteService
    {
        public IMethodInvocationResult Invoke(IMessage message)
        {
            throw new System.NotImplementedException();
            //use registry, get slave, delegate calculation, store data
        }
    }
}