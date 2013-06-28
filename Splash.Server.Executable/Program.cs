using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splash.Common;
using Splash.RemoteServiceContract;

namespace Splash.Server.Executable
{
    class Program
    {
        static void Main(string[] args)
        {
            var uri = new Uri("http://localhost:7000/Master");
            ServiceExecutable serviceExecutable = new ServiceExecutable(typeof(IRemoteService), typeof(MasterServer), uri);
            serviceExecutable.InitialiseService();
            Console.ReadLine();
        }
    }
}
