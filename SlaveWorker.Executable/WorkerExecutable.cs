using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Description;
using Splash.RemoteServiceContract;
using Splash.SlaveWorker.Executable.ServiceRegistry;
using Splash.SlaveWorker.Interfaces;

namespace Splash.SlaveWorker.Executable
{
    public class WorkerExecutable
    {
        private QueueSnsor _queueSensor;
        private ServiceHost _host;

        public string Id { get; private set; }
        public string WorkerUri { get; private set; }

        public void Initialise()
        {
            Tuple<string, string> id = RegisterService();
            Id = id.Item2;
            Uri uri = Connect(Id);
            WorkerUri = uri.ToString();
            CompleteRegistrationProcess(uri);
            string hostName = new Uri(id.Item1).Host;
            IPAddress ip = Dns.GetHostAddresses(hostName)[0];
            _queueSensor = new QueueSnsor(5000, Id);
            _queueSensor.Connect(ip.ToString(), 8001);
        }

        private Tuple<string, string> RegisterService()
        {
            using (var serviceRegistry = new RegistryClient())
            {
                return Tuple.Create(serviceRegistry.Endpoint.Address.ToString(), serviceRegistry.AssignServerId());
            }
        }

        private Uri Connect(string serviceId)
        {
            //Create a URI to serve as the base address
            Uri httpUrl = DetermineWorkerUri(serviceId);
            Console.WriteLine("Worker started: " + httpUrl);
            //Create ServiceHost
            _host = new ServiceHost(typeof(Worker), httpUrl);
            //Add a service endpoint
            var binding = new WSHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            _host.AddServiceEndpoint(typeof(IRemoteService), binding, httpUrl);
            //Enable metadata exchange
            var smb = new ServiceMetadataBehavior { HttpGetEnabled = true };
            _host.Description.Behaviors.Add(smb);
            //Start the Service
            _host.Open();

            return httpUrl;
        }

        private Uri DetermineWorkerUri(string serviceId)
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            return
                new Uri("http://" + host.AddressList.First(p => p.AddressFamily == AddressFamily.InterNetwork) +
                        ":8090/Worker/" + serviceId + "/");
        }

        private void CompleteRegistrationProcess(Uri serverUri)
        {
            using (var serviceRegistry = new RegistryClient())
            {
                serviceRegistry.AcknowlegdeRegistration(Id, serverUri.ToString());
            }
        }
    }
}