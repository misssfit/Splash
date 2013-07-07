using System;
using System.Collections.Generic;
using System.Threading;

namespace Splash.Common
{
    public abstract class ActiveObject
    {
        public ActiveObject() { Exceptions = new List<Exception>(); }

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

        public void Join()
        {
            try
            {
                _thread.Join();

            }
            catch (Exception e)
            {

                Exceptions.Add(e);
            }
        }

        public List<Exception> Exceptions { get; protected set; }
    }
}