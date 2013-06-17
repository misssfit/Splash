using System;

namespace SlaveWorker.Executable
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var worker = new WorkerExecutable();
            worker.Initialise();
            Console.ReadLine();
        }
    }
}