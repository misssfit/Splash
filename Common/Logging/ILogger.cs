namespace Splash.Common.Logging
{
    public interface ILogger
    {
        void LogWarning(string message);
        void LogInfo(string message);
        void LogError(string message);

        void Log(string message, LogLevel logLevel);

    }
}