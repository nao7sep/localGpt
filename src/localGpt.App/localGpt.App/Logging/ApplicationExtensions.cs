using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using localGpt.App.Logging.Views;
using System;

namespace localGpt.App.Logging
{
    /// <summary>
    /// Extension methods for the Application class.
    /// </summary>
    public static class ApplicationExtensions
    {
        /// <summary>
        /// Shows an error dialog with the specified message.
        /// </summary>
        /// <param name="app">The application instance</param>
        /// <param name="message">The error message to display</param>
        /// <param name="owner">The owner window (optional)</param>
        public static void ShowErrorDialog(this Application app, string message, Window? owner = null)
        {
            try
            {
                // For UI thread exceptions, we can show the dialog directly
                if (Dispatcher.UIThread.CheckAccess())
                {
                    var dialog = new ErrorDialog(message, owner);
                    if (owner != null)
                    {
                        // Show as dialog if we have an owner window
                        _ = dialog.ShowDialog(owner);
                    }
                    else
                    {
                        // Otherwise just show as a window
                        dialog.Show();
                    }
                }
                else
                {
                    // For non-UI thread exceptions, we need to dispatch to the UI thread
                    Dispatcher.UIThread.Post(() => ShowErrorDialog(app, message, owner));
                }
            }
            catch (Exception ex)
            {
                // If we can't show the error dialog, log the error
                Logger.Error(ex, "Failed to show error dialog: {Message}", ex.Message);
            }
        }
    }
}