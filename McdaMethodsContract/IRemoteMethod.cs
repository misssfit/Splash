using System.Collections.Generic;
using System.Xml.Linq;
using Splash.RemoteServiceContract;

namespace Splash.RemoteMethodsContract
{
    public interface IRemoteMethod
    {
        RemoteMethodInfo MethodMetadata { get; }
        XElement Calculate(List<ObjectParameter> _inputParameters);
    }
}