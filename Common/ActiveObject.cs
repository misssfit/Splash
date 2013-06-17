using System.Threading;

namespace Common
{
    public abstract class ActiveObject
    {
        protected Thread _thread;
        public bool IsActive { get; protected set; }

        public void Run()
        {
            IsActive = true;
            _thread = new Thread(PerformAction);
            _thread.Start();
        }

        public void Stop()
        {
            IsActive = false;
            _thread.Abort();
        }

        protected abstract void PerformAction();
    }
}