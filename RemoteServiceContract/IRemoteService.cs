using System.ServiceModel;

namespace Splash.RemoteServiceContract
{
    [ServiceContract]
    public interface IRemoteService
    {
        [OperationContract]
        OperationStatus Invoke(RemoteMessage message);

        //[OperationContract]
        //List<MethodDescription> GetAllMethods();


        [OperationContract]
        CalculationResult GetResult(string id);


        [OperationContract]
        OperationStatus DeleteTask(string id);
    }
}