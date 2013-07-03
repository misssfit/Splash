using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Splash.Common;

namespace Splash.ServiceRegistry
{
    public class SlaveRegistry : Singleton<SlaveRegistry>
    {
        protected List<SlaveServerMetadata> _registredServers;
        private readonly object _lock = new object();
        private Timer _cleanRegistryTimer;
        private List<string> _recentlyRemovedWorkers;
 
        public SlaveRegistry()
        {
            _registredServers = new List<SlaveServerMetadata>();
            _cleanRegistryTimer = TimerBasedObject.InitializeNewTimer(OptimizeRegistry, 60000);
            _recentlyRemovedWorkers = new List<string>();

        }

        private void OptimizeRegistry(object sender, ElapsedEventArgs e)
        {
            lock (_lock)
            {
                if (_registredServers.Any() == true)
                {
                    _registredServers.RemoveAll(p => p.Status == WorkerStatus.WaitingToBeRemoved);

                    foreach (var slaveServerMetadata in _registredServers)
                    {
                        switch (slaveServerMetadata.Status)
                        {
                            case WorkerStatus.Faulted:
                                slaveServerMetadata.Status = WorkerStatus.DisconnectionConfirmed;
                                break;

                            case WorkerStatus.DisconnectionConfirmed:
                                slaveServerMetadata.Status = WorkerStatus.WaitingToBeRemoved;
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
        }

        public string ReserveId()
        {
            lock (_lock)
            {
                string id = Guid.NewGuid().ToString();
                _registredServers.Add(new SlaveServerMetadata { Id = id, Status = WorkerStatus.New });
                return id;
            }
        }

        public void AssignUriToServer(string serviceId, string uri)
        {
            lock (_lock)
            {
                SlaveServerMetadata server = _registredServers.Single(p => p.Id == serviceId);
                server.Uri = uri;
                server.Status = WorkerStatus.Registred;
            }
        }

        public void UpdateServiceStatus(Measurement measurement)
        {
            lock (_lock)
            {
                SlaveServerMetadata server = _registredServers.Single(p => p.Id == measurement.ResourceId);
                server.QueueSize = int.Parse(measurement.Value);
                if (server.Status != WorkerStatus.Connected)
                {
                    server.Status = WorkerStatus.Connected;

                }
            }
        }

        public string AssignServer()
        {
            lock (_lock)
            {
                return _registredServers.First().Uri;
            }
        }

        internal void SetWorkerStatus(string resourceId, WorkerStatus workerStatus)
        {
            lock (_lock)
            {
                SlaveServerMetadata server = _registredServers.Single(p => p.Id == resourceId);
                server.Status = workerStatus;
            }
        }

        public List<string> GetInactiveWorkers()
        {
            lock (_lock)
            {
                var inactive = _recentlyRemovedWorkers.Select(p => p).ToList();
                _recentlyRemovedWorkers.Clear();
                return inactive;
            }
        }
    }
}