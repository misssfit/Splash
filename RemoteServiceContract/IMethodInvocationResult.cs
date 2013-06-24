

namespace Splash.RemoteServiceContract
{
    public interface IMethodInvocationResult
    {
        string Id { get; set; }
        RequestStatus Status { get; set; }

    }
}