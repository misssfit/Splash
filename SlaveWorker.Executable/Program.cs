using System;
using System.Linq;

namespace Splash.SlaveWorker.Executable
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var worker = new WorkerExecutable();
            worker.Initialise();

            worker.Join();

            while (worker.Exceptions.Any() == true)
            {
               // worker.Reconnect();
                worker = new WorkerExecutable();
                worker.Initialise();
                worker.Join();
            }
        }
    }
}