using System;
using Splash.Common;
using Splash.RemoteMethodsContract;
using Splash.RemoteServiceContract;

namespace Splash.SlaveWorker
{
    class MethodResolver : Singleton<MethodResolver>
    {

        internal Lazy<IRemoteMethod> Resolve(string methodName)
        {
            return new Lazy<IRemoteMethod>(() => { return MethodRegistry.Instance.GetMethodObject(methodName); });


        }
    }
}
