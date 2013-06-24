using Splash.RemoteServiceContract;

namespace Splash.SlaveWorker.Data
{
    public class OperationStatus : IMethodInvocationResult
    {
        public string Id { get; set; }
        public RemoteServiceContract.RequestStatus Status { get; set; }
    }
}