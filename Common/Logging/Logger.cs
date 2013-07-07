using System;

namespace Splash.Common.Logging
{
    public class Logger : Singleton<Logger>, ILogger
    {
        public void LogWarning(string message)
        {
            Log(message, LogLevel.Warning);
        }

        public void LogInfo(string message)
        {
            Log(message, LogLevel.Info);
        }

        public void LogError(string message)
        {
            Log(message, LogLevel.Error);
        }

        public void Log(string message, LogLevel logLevel)
        {
            Console.WriteLine("{0} | {1} | {2}", DateTime.Now, message, logLevel.ToString());
        }
    }
}