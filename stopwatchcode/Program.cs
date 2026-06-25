namespace StopwatchApp.Core
{
    /// <summary>
    /// Application entry point. Configures Windows Forms and launches the
    /// main stopwatch window.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The method where the program starts running.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new StopwatchForm());
        }
    }
}
