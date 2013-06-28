using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Splash.Common
{
    public class ServiceExecutable 
    {
        public Uri ServiceUri { get; private set; }
        private Type _serviceContract;
        private Type _serviceImplementation;
        public ServiceHost Host { get; private set; }

        public ServiceExecutable(Type serviceContract, Type serviceImplementation, Uri uri)
        {
            _serviceContract = serviceContract;
            _serviceImplementation = serviceImplementation;
            ServiceUri = uri;
        }

        public void InitialiseService()
        {
            Host = new ServiceHost(_serviceImplementation, ServiceUri);
            //Add a service endpoint
            var binding = new WSHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            Host.AddServiceEndpoint(_serviceContract, binding, "");
            //Enable metadata exchange
            var smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            Host.Description.Behaviors.Add(smb);
            //Start the Service
            Host.Open();
            Console.WriteLine("Service started: " + ServiceUri);
        }
    }
}