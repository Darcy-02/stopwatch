using System;
using System.Timers;
using Timer = System.Timers.Timer;

namespace StopwatchApp.Core
{
    public enum StopwatchState
    {
        Idle,
        Running,
        Paused,
        Stopped
    }

    public class StopwatchEngine : IDisposable
    {

        private readonly Timer _timer;         
        private int _totalSeconds;              
        private StopwatchState _state;
        private bool _disposed;

        public event EventHandler<string>? TimerTick;
        public event EventHandler<StopwatchState>? StateChanged;

        public StopwatchEngine()
        {
            _timer = new Timer(1000); 
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true;
            _state = StopwatchState.Idle;
            _totalSeconds = 0;
        }

        public StopwatchState State => _state;

        public int TotalSeconds => _totalSeconds;

        public string FormattedTime => FormatTime(_totalSeconds);

        public void Start()
        {
            if (_state == StopwatchState.Running)
                throw new InvalidOperationException("stopwatch is running already");

            if (_state == StopwatchState.Paused)
                throw new InvalidOperationException("stopwatch is paused");

            _totalSeconds = 0;
            _timer.Start();
            SetState(StopwatchState.Running);
        }

        public string Pause()
        {
            if (_state != StopwatchState.Running)
                throw new InvalidOperationException("stopwatch is paused");

            _timer.Stop();
            SetState(StopwatchState.Paused);
            return FormattedTime;
        }


        public void Resume()
        {
            if (_state != StopwatchState.Paused)
                throw new InvalidOperationException("Stopwatch is resumed");

            _timer.Start();
            SetState(StopwatchState.Running);
        }

       
        public void Reset()
        {
            _timer.Stop();
            _totalSeconds = 0;
            SetState(StopwatchState.Idle);
            // Notify subscribers of the zeroed display
            TimerTick?.Invoke(this, FormattedTime);
        }

        
        public string Stop()
        {
            if (_state != StopwatchState.Running && _state != StopwatchState.Paused)
                throw new InvalidOperationException("stopwatch is has stopped");

            _timer.Stop();
            SetState(StopwatchState.Stopped);
            return FormattedTime;
        }


        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            _totalSeconds++;
            TimerTick?.Invoke(this, FormattedTime);
        }


        private void SetState(StopwatchState newState)
        {
            _state = newState;
            StateChanged?.Invoke(this, _state);
        }


        public static string FormatTime(int totalSeconds)
        {
            if (totalSeconds < 0)
                throw new ArgumentOutOfRangeException(nameof(totalSeconds), "value must be non-negative.");

            int hours   = totalSeconds / 3600;
            int minutes = (totalSeconds % 3600) / 60;
            int seconds = totalSeconds % 60;

            return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                _timer.Stop();
                _timer.Dispose();
            }
            _disposed = true;
        }
    }
}

