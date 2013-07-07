using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Timers;
using Splash.Common;
using Splash.Common.Logging;
using Splash.Server.RegistryServiceReference;

namespace Splash.Server
{
    public class DelegatedTaskStorage : Singleton<DelegatedTaskStorage>
    {
        private object _lock = new object();
        private StringDictionary _dictionary = new StringDictionary();
        private Timer _synchronizeInactiveWorkers;
        private Timer _cleanInactiveWorkers;

        private List<string> _inactiveWorkers;

        public DelegatedTaskStorage()
        {
            _synchronizeInactiveWorkers = TimerBasedObject.InitializeNewTimer(SynchronizeInactiveWorkers, 60000);
            _cleanInactiveWorkers = TimerBasedObject.InitializeNewTimer(CleanInactiveWorkers, 600000);
            _inactiveWorkers = new List<string>();
        }

        private void CleanInactiveWorkers(object sender, ElapsedEventArgs e)
        {
            lock (_inactiveWorkers)
            {//todo: remeve based on datetime
                Logger.Instance.LogInfo(string.Format("Inactive workers base cleaned with Registry. {0} removed", _inactiveWorkers.Count));
                _inactiveWorkers.Clear();


            }
        }

        private void SynchronizeInactiveWorkers(object sender, ElapsedEventArgs e)
        {
            try
            {
                using (var registry = new RegistryClient())
                {
                    var inactive = registry.SynchronizeInactiveWorkers().ToList();
                    lock (_inactiveWorkers)
                    {
                        Logger.Instance.LogInfo(string.Format("Inactive workers synchronized with Registry. {0} found", inactive.Count));

                        _inactiveWorkers.AddRange(inactive);
                        _inactiveWorkers = _inactiveWorkers.Distinct().ToList();
                    }
                }
            }
            catch
            {
            }
        }

        public void Add(string id, string serverUri)
        {
            lock (_lock)
            {
                if (_dictionary.ContainsKey(id) == false)
                {
                    _dictionary.Add(id, serverUri);
                }
            }
        }

        internal string GetAssignedWorker(string id)
        {
            lock (_lock)
            {
                lock (_inactiveWorkers)
                {
                    if (_inactiveWorkers.Contains(id) == true)
                    {
                        return null;
                    }
                }

                if (_dictionary.ContainsKey(id) == true)
                {
                    return _dictionary[id];
                }
            }
            return null;
        }

        internal void RemoveAssignedWorker(string id)
        {
            lock (_lock)
            {
                if (_dictionary.ContainsKey(id) == true)
                {
                    Logger.Instance.LogInfo(string.Format("Assigned Worker removed from dictionary: {0}", id));

                    _dictionary.Remove(id);
                }
            }
        }
    }
}