using System.Collections.Generic;
using Splash.RemoteServiceContract;

namespace Splash.RemoteMethodsContract
{
    public interface IRemoteMethod
    {
        RemoteMethodInfo MethodMetadata { get; }
        double[][] Calculate(List<ObjectParameter> _inputParameters);
    }
}