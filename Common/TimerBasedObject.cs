using System.Timers;

namespace Splash.Common
{
    public abstract class TimerBasedObject
    {
        protected Timer _timer;

        protected TimerBasedObject(int interval = 1000)
        {
            _timer = new Timer();
            _timer.Elapsed += OnTick;
            _timer.Interval = interval;
            _timer.Enabled = true;
            _timer.Start();
        }

        protected abstract void OnTick(object sender, ElapsedEventArgs e);

        public static Timer InitializeNewTimer(ElapsedEventHandler onTick, int interval)
        {
            var timer = new Timer();
            timer.Elapsed += onTick;
            timer.Interval = interval;
            timer.Enabled = true;
            timer.Start();
            return timer;
        }
    }
}