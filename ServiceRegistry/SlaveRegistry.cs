using System;
using System.Collections.Generic;
using System.Linq;
using Splash.Common;

namespace Splash.ServiceRegistry
{
    public class SlaveRegistry : Singleton<SlaveRegistry>
    {
        protected List<SlaveServerMetadata> _registredServers;
        private readonly object _lock = new object();

        public SlaveRegistry()
        {
            _registredServers = new List<SlaveServerMetadata>();
        }

        public string ReserveId()
        {
            lock (_lock)
            {
                string id = Guid.NewGuid().ToString();
                _registredServers.Add(new SlaveServerMetadata {Id = id});
                return id;
            }
        }

        public void AssignUriToServer(string serviceId, string uri)
        {
            lock (_lock)
            {
                SlaveServerMetadata server = _registredServers.Single(p => p.Id == serviceId);
                server.Uri = uri;
            }
        }

        public void UpdateServiceStatus(Measurement measurement)
        {
            lock (_lock)
            {
                SlaveServerMetadata server = _registredServers.Single(p => p.Id == measurement.ResourceId);
                server.QueueSize = int.Parse(measurement.Value);
            }
        }
    }
}