using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using Splash.Common;

namespace Splash.ServiceRegistry.Executable
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Create a URI to serve as the base address
            var httpUrl = new Uri("http://localhost:8000/Registry");
            ServiceExecutable serviceExecutable = new ServiceExecutable(typeof(IRegistry), typeof(Registry), httpUrl);
            serviceExecutable.InitialiseService();
            Console.ReadLine();
        }
    }
}