namespace Splash.RemoteServiceContract
{
    public interface IRemoteMethod
    {
        IResult Invoke(IMessage message);
    }
}