namespace Splash.MeasurementSensor
{
    internal interface ISensor
    {
        void Connect(string host, int port);
        void Disconnect();
    }
}