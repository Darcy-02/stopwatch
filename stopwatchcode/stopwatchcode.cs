using System;
using System.Timers;
using Timer = System.Timers.Timer;

namespace StopwatchApp.Core
{
    /// <summary>
    /// The possible states the stopwatch can be in at any moment.
    /// </summary>
    public enum StopwatchState
    {
        /// <summary>The stopwatch has not been started, or has been reset.</summary>
        Idle,

        /// <summary>The stopwatch is currently counting up.</summary>
        Running,

        /// <summary>The stopwatch is temporarily paused.</summary>
        Paused,

        /// <summary>The stopwatch has been stopped.</summary>
        Stopped
    }

    /// <summary>
    /// Handles all stopwatch timing and state logic. It counts elapsed seconds
    /// using a one-second timer and raises events so a user interface can
    /// update without containing any timing logic itself.
    /// </summary>
    public class StopwatchEngine : IDisposable
    {
        /// <summary>The timer that fires once every second.</summary>
        private readonly Timer _timer;

        /// <summary>The total number of seconds counted so far.</summary>
        private int _totalSeconds;

        /// <summary>The current state of the stopwatch.</summary>
        private StopwatchState _state;

        /// <summary>Tracks whether this object has already been disposed.</summary>
        private bool _disposed;

        /// <summary>
        /// Raised every second with the current time as a formatted string,
        /// so the UI can refresh its display.
        /// </summary>
        public event EventHandler<string>? TimerTick;

        /// <summary>
        /// Raised whenever the stopwatch changes state, so the UI can react
        /// (for example, by enabling or disabling buttons).
        /// </summary>
        public event EventHandler<StopwatchState>? StateChanged;

        /// <summary>
        /// Creates a new stopwatch engine in the Idle state with the timer set
        /// to tick once per second.
        /// </summary>
        public StopwatchEngine()
        {
            _timer = new Timer(1000);
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true;
            _state = StopwatchState.Idle;
            _totalSeconds = 0;
        }

        /// <summary>Gets the current state of the stopwatch.</summary>
        public StopwatchState State => _state;

        /// <summary>Gets the total elapsed seconds counted so far.</summary>
        public int TotalSeconds => _totalSeconds;

        /// <summary>Gets the elapsed time formatted as <c>hh:mm:ss</c>.</summary>
        public string FormattedTime => FormatTime(_totalSeconds);

        /// <summary>
        /// Starts the stopwatch from <c>00:00:00</c>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the stopwatch is already running or is paused.
        /// </exception>
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

        /// <summary>
        /// Pauses the stopwatch and returns the current time.
        /// </summary>
        /// <returns>The current elapsed time formatted as <c>hh:mm:ss</c>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the stopwatch is not currently running.
        /// </exception>
        public string Pause()
        {
            if (_state != StopwatchState.Running)
                throw new InvalidOperationException("stopwatch is paused");

            _timer.Stop();
            SetState(StopwatchState.Paused);
            return FormattedTime;
        }

        /// <summary>
        /// Resumes the stopwatch from the time at which it was paused.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the stopwatch is not currently paused.
        /// </exception>
        public void Resume()
        {
            if (_state != StopwatchState.Paused)
                throw new InvalidOperationException("Stopwatch is resumed");

            _timer.Start();
            SetState(StopwatchState.Running);
        }

        /// <summary>
        /// Resets the stopwatch back to <c>00:00:00</c> and returns it to the
        /// Idle state. Notifies subscribers so the display shows zero.
        /// </summary>
        public void Reset()
        {
            _timer.Stop();
            _totalSeconds = 0;
            SetState(StopwatchState.Idle);
            // Notify subscribers of the zeroed display
            TimerTick?.Invoke(this, FormattedTime);
        }

        /// <summary>
        /// Stops the stopwatch completely and returns the last recorded time.
        /// </summary>
        /// <returns>The final elapsed time formatted as <c>hh:mm:ss</c>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the stopwatch is neither running nor paused.
        /// </exception>
        public string Stop()
        {
            if (_state != StopwatchState.Running && _state != StopwatchState.Paused)
                throw new InvalidOperationException("stopwatch is has stopped");

            _timer.Stop();
            SetState(StopwatchState.Stopped);
            return FormattedTime;
        }

        /// <summary>
        /// Runs once per second while the timer is active. Increments the
        /// elapsed seconds and notifies subscribers of the new time.
        /// </summary>
        /// <param name="sender">The timer that raised the event.</param>
        /// <param name="e">The elapsed-event data.</param>
        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            _totalSeconds++;
            TimerTick?.Invoke(this, FormattedTime);
        }

        /// <summary>
        /// Updates the current state and notifies subscribers of the change.
        /// </summary>
        /// <param name="newState">The new state to switch to.</param>
        private void SetState(StopwatchState newState)
        {
            _state = newState;
            StateChanged?.Invoke(this, _state);
        }

        /// <summary>
        /// Converts a number of seconds into an <c>hh:mm:ss</c> string.
        /// </summary>
        /// <param name="totalSeconds">The total seconds to format.</param>
        /// <returns>The time formatted as <c>hh:mm:ss</c> with zero-padding.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="totalSeconds"/> is negative.
        /// </exception>
        public static string FormatTime(int totalSeconds)
        {
            if (totalSeconds < 0)
                throw new ArgumentOutOfRangeException(nameof(totalSeconds), "value must be non-negative.");

            int hours   = totalSeconds / 3600;
            int minutes = (totalSeconds % 3600) / 60;
            int seconds = totalSeconds % 60;

            return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
        }

        /// <summary>
        /// Releases the resources used by the stopwatch (the underlying timer).
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases resources used by the stopwatch.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> when called directly; stops and disposes the timer.
        /// </param>
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
