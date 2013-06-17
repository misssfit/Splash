using System.ServiceModel;

namespace SlaveWorker
{
    [ServiceContract]
    public interface IWorker
    {
        [OperationContract]
        void Dmmy();
    }
}