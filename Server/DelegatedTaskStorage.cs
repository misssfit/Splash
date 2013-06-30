using System.Collections.Specialized;
using Splash.Common;

namespace Splash.Server
{
    public class DelegatedTaskStorage : Singleton<DelegatedTaskStorage>
    {
        private object _lock = new object();
        private StringDictionary _dictionary = new StringDictionary();

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
                    _dictionary.Remove(id);
                }
            }
        }
    }
}