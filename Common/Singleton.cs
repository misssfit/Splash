namespace Common
{
    public class Singleton<T> where T : class, new()
    {
        private static readonly object _syncLock = new object();
        private static T _instance;

        protected Singleton()
        {
        }

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new T();
                        }
                    }
                }
                return _instance;
            }
        }
    }
}