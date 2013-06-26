using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Splash.RemoteServiceContract
{
    public class RemoteMessage
    {
        public string MethodName { get; set; }
        public List<ObjectParameter> MethodParameters { get; set; }
        public ResultReturnMethod ResultReturnMethod { get; set; }

        public RemoteMessage()
        {
            ResultReturnMethod = ResultReturnMethod.Memory;
        }
    }
}