using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace ServiceRegistry.Executable
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Create a URI to serve as the base address
            var httpUrl = new Uri("http://localhost:8000/Registry");
            //Create ServiceHost
            var host = new ServiceHost(typeof (Registry), httpUrl);
            //Add a service endpoint
            var binding = new WSHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            host.AddServiceEndpoint(typeof (IRegistry), binding, "");
            //Enable metadata exchange
            var smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            host.Description.Behaviors.Add(smb);
            //Start the Service
            host.Open();
        }
    }
}