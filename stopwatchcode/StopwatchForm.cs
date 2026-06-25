using System.Drawing;
using System.Windows.Forms;

namespace StopwatchApp.Core
{
    /// <summary>
    /// The graphical window for the stopwatch. It displays the elapsed time and
    /// provides Start, Pause, Resume, Reset, and Stop buttons. All timing logic
    /// lives in <see cref="StopwatchEngine"/>; this form only shows the time and
    /// forwards button clicks to the engine.
    /// </summary>
    public class StopwatchForm : Form
    {
        /// <summary>The engine that does the actual timing and state handling.</summary>
        private readonly StopwatchEngine _engine = new();

        private readonly Label _timeLabel = new();
        private readonly Label _statusLabel = new();
        private readonly Button _startButton = new();
        private readonly Button _pauseButton = new();
        private readonly Button _resumeButton = new();
        private readonly Button _resetButton = new();
        private readonly Button _stopButton = new();

        /// <summary>
        /// Builds the window, lays out the controls, and subscribes to the
        /// engine's events.
        /// </summary>
        public StopwatchForm()
        {
            Text = "Stopwatch";
            Width = 470;
            Height = 290;
            Font = new Font("Segoe UI", 10);
            FormClosed += (s, e) => _engine.Dispose();   // clean up the timer on close

            // Big time display.
            _timeLabel.Text = "00:00:00";
            _timeLabel.Font = new Font("Consolas", 48, FontStyle.Bold);
            _timeLabel.TextAlign = ContentAlignment.MiddleCenter;
            _timeLabel.SetBounds(20, 20, 420, 90);

            // Status line (e.g. "Paused at 00:00:05").
            _statusLabel.Text = "Ready";
            _statusLabel.TextAlign = ContentAlignment.MiddleCenter;
            _statusLabel.SetBounds(20, 115, 420, 25);

            // Five buttons in a row.
            ConfigureButton(_startButton,  "Start",  20,  StartClick);
            ConfigureButton(_pauseButton,  "Pause",  105, PauseClick);
            ConfigureButton(_resumeButton, "Resume", 190, ResumeClick);
            ConfigureButton(_resetButton,  "Reset",  275, ResetClick);
            ConfigureButton(_stopButton,   "Stop",   360, StopClick);

            Controls.Add(_timeLabel);
            Controls.Add(_statusLabel);

            // The engine raises these events; they may fire on a timer thread.
            _engine.TimerTick += OnTimerTick;
            _engine.StateChanged += OnStateChanged;

            UpdateButtons(_engine.State);
        }

        /// <summary>Sets up a button's text, position, and click handler.</summary>
        /// <param name="button">The button to configure.</param>
        /// <param name="text">The label shown on the button.</param>
        /// <param name="left">The horizontal position in pixels.</param>
        /// <param name="onClick">The method to run when the button is clicked.</param>
        private void ConfigureButton(Button button, string text, int left, EventHandler onClick)
        {
            button.Text = text;
            button.SetBounds(left, 160, 75, 45);
            button.Click += onClick;
            Controls.Add(button);
        }

        /// <summary>Starts the stopwatch from 00:00:00.</summary>
        private void StartClick(object? sender, EventArgs e) =>
            Run(() => { _engine.Start(); _statusLabel.Text = "Running"; });

        /// <summary>Pauses the stopwatch and shows the current time.</summary>
        private void PauseClick(object? sender, EventArgs e) =>
            Run(() => _statusLabel.Text = "Paused at " + _engine.Pause());

        /// <summary>Resumes the stopwatch from the paused time.</summary>
        private void ResumeClick(object? sender, EventArgs e) =>
            Run(() => { _engine.Resume(); _statusLabel.Text = "Running"; });

        /// <summary>Resets the stopwatch back to 00:00:00.</summary>
        private void ResetClick(object? sender, EventArgs e) =>
            Run(() => { _engine.Reset(); _statusLabel.Text = "Reset"; });

        /// <summary>Stops the stopwatch and shows the last recorded time.</summary>
        private void StopClick(object? sender, EventArgs e) =>
            Run(() => _statusLabel.Text = "Stopped at " + _engine.Stop());

        /// <summary>
        /// Runs an engine action, turning any invalid-state error into a status
        /// message instead of letting the program crash.
        /// </summary>
        /// <param name="action">The engine operation to attempt.</param>
        private void Run(Action action)
        {
            try
            {
                action();
            }
            catch (InvalidOperationException ex)
            {
                _statusLabel.Text = ex.Message;
            }
        }

        /// <summary>
        /// Updates the time label every second. Marshals back onto the UI thread
        /// because the engine's timer fires on a background thread.
        /// </summary>
        private void OnTimerTick(object? sender, string time)
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => _timeLabel.Text = time);
                return;
            }
            _timeLabel.Text = time;
        }

        /// <summary>
        /// Enables or disables buttons whenever the engine's state changes, so
        /// only valid actions are clickable.
        /// </summary>
        private void OnStateChanged(object? sender, StopwatchState state)
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => UpdateButtons(state));
                return;
            }
            UpdateButtons(state);
        }

        /// <summary>
        /// Turns each button on or off to match the current stopwatch state.
        /// </summary>
        /// <param name="state">The engine's current state.</param>
        private void UpdateButtons(StopwatchState state)
        {
            _startButton.Enabled  = state is StopwatchState.Idle or StopwatchState.Stopped;
            _pauseButton.Enabled  = state == StopwatchState.Running;
            _resumeButton.Enabled = state == StopwatchState.Paused;
            _resetButton.Enabled  = state != StopwatchState.Idle;
            _stopButton.Enabled   = state is StopwatchState.Running or StopwatchState.Paused;
        }
    }
}
