using System.Collections.Generic;

namespace Splash.RemoteServiceContract
{
    public interface IMessage
    {
        string MethodName { get; set; }
        List<IParameter> MethodParameters { get; set; }
    }
}