using System.Collections.Generic;

namespace Splash.RemoteServiceContract
{
    public class RemoteMessage : IMessage
    {
        public string MethodName { get; set; }
        public List<IParameter> MethodParameters { get; set; }
    }
}