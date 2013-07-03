using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using Splash.Common;
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
            _queueSensor = new QueueSnsor(10000, Id);
            _queueSensor.Connect(ip.ToString(), 8001);
        }

        private Tuple<string, string> RegisterService()
        {
            var id = string.Empty;
            var endpoint = string.Empty;

            while (string.IsNullOrWhiteSpace(id) == true)
            {
                try
                {
                    using (var serviceRegistry = new RegistryClient())
                    {
                        endpoint = serviceRegistry.Endpoint.Address.ToString();
                        id = serviceRegistry.AssignServerId();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Cannot Register Worker. Retrying...");
                    Console.WriteLine("Exception details: " + e.Message);
                }
            }
            return Tuple.Create(endpoint, id);
        }

        private Uri Connect(string serviceId)
        {
            Uri httpUrl = DetermineWorkerUri(serviceId);

            ServiceExecutable serviceExecutable = new ServiceExecutable(typeof(IRemoteService), typeof(Worker), httpUrl);
            serviceExecutable.InitialiseService();
            _host = serviceExecutable.Host;
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
            var result = false;
            while (result == false)
            {
                try
                {
                    using (var serviceRegistry = new RegistryClient())
                    {
                        result = serviceRegistry.AcknowlegdeRegistration(Id, serverUri.ToString());
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Cannot AcknowlegdeRegistration. Retrying...");
                    Console.WriteLine("Exception details: " + e.Message);
                }

            }
        }
    }
}