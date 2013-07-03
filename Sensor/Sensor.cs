using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
using System.Web.Script.Serialization;
using Splash.Common;
using Timer = System.Timers.Timer;

namespace Splash.MeasurementSensor
{
    public abstract class Sensor : ActiveObject, ISensor
    {
        private readonly JavaScriptSerializer _javaScriptSerializer = new JavaScriptSerializer();
        protected Timer _clock;
        protected IPEndPoint _hostEndPoint;
        protected string _hostName;

        private Socket _socket;
        private readonly object _lock = new object();
        private readonly Queue<string> _queue = new Queue<string>();
        private int _sleepResolution;
        public Sensor()
            : this(1000, "")
        {
        }

        public Sensor(int resolution, string hostName)
        {
            _sleepResolution = resolution / 2;
            if (string.IsNullOrWhiteSpace(hostName) == true)
            {
                _hostName = Environment.MachineName;
            }
            else
            {
                _hostName = hostName;
            }
            Resolution = resolution;
            _clock = new Timer();
            _clock.Interval = Resolution;
            _clock.Elapsed += Run;
            _clock.Enabled = false;
        }

        public bool IsWorking { get; set; }
        public int Resolution { get; set; }

        public void Connect(string host, int port)
        {
            _clock.Enabled = true;
            _clock.Start();
            IsWorking = true;
            _hostEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect("127.0.0.1", port);
            Run();

        }

        public void Disconnect()
        {
            _clock.Stop();

            _socket.Close();
        }

        protected void Send(Measurement measurement)
        {
            lock (_lock)
            {
                string jsonObject = _javaScriptSerializer.Serialize(measurement);
                _queue.Enqueue(jsonObject);
            }
        }

        protected void Run(object state, ElapsedEventArgs args)
        {
            Measurement measurement = Measure();
            Send(measurement);
        }

        protected abstract Measurement Measure();

        protected override void PerformAction()
        {
            while (IsActive)
            {
                lock (_lock)
                {
                    if (_socket != null)
                    {
                        while (_queue.Count > 0)
                        {

                            try
                            {
                                byte[] byteBuffer = Encoding.ASCII.GetBytes(_queue.Dequeue());
                                _socket.Send(byteBuffer);
                            }
                            catch
                            {
                                Console.WriteLine("Exception reading from Server");
                                throw new Exception();//ServerDisconnectedException();
                            }
                        }
                    }
                }
                Thread.Sleep(_sleepResolution);
            }

        }
    }
}