using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splash.RemoteServiceContract;

namespace ClientCommunicationLayer
{
    public class RemoteConnector
    {
        public TReturn InvokeRemoteMethod<TReturn>(string methodName, params ObjectParameter[] inputParameters)
        {
            return default(TReturn);
        }
    }
}
