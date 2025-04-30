using Avalonia.Controls;
using Avalonia.VisualTree;
using localGpt.App.Logging.Views;
using System;
using System.Threading.Tasks;

namespace localGpt.App.Logging
{
    /// <summary>
    /// Provides methods for handling exceptions consistently throughout the application.
    /// </summary>
    public static class ExceptionHandler
    {
        /// <summary>
        /// Handles an exception by logging it and optionally showing a message to the user.
        /// </summary>
        /// <param name="exception">The exception to handle</param>
        /// <param name="context">Context information about where the exception occurred</param>
        /// <param name="showToUser">Whether to show a message to the user</param>
        /// <param name="parentWindow">The parent window for the message dialog</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public static async Task HandleExceptionAsync(Exception exception, string context, bool showToUser = true, Window? parentWindow = null)
        {
            // Log the exception
            Logger.Error(exception, "Exception in {Context}: {Message}", context, exception.Message);

            // Show a message to the user if requested
            if (showToUser)
            {
                await ShowErrorMessageAsync(exception.Message, parentWindow);
            }
        }

        /// <summary>
        /// Executes an action and handles any exceptions that occur.
        /// </summary>
        /// <param name="action">The action to execute</param>
        /// <param name="context">Context information about where the action is being executed</param>
        /// <param name="showToUser">Whether to show a message to the user if an exception occurs</param>
        /// <param name="parentWindow">The parent window for the message dialog</param>
        public static void Execute(Action action, string context, bool showToUser = true, Window? parentWindow = null)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                // Fire and forget the async exception handling
                _ = HandleExceptionAsync(ex, context, showToUser, parentWindow);
            }
        }

        /// <summary>
        /// Executes a function and handles any exceptions that occur.
        /// </summary>
        /// <typeparam name="T">The return type of the function</typeparam>
        /// <param name="func">The function to execute</param>
        /// <param name="context">Context information about where the function is being executed</param>
        /// <param name="defaultValue">The default value to return if an exception occurs</param>
        /// <param name="showToUser">Whether to show a message to the user if an exception occurs</param>
        /// <param name="parentWindow">The parent window for the message dialog</param>
        /// <returns>The result of the function or the default value if an exception occurs</returns>
        public static T Execute<T>(Func<T> func, string context, T defaultValue, bool showToUser = true, Window? parentWindow = null)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                // Fire and forget the async exception handling
                _ = HandleExceptionAsync(ex, context, showToUser, parentWindow);
                return defaultValue;
            }
        }

        /// <summary>
        /// Executes an asynchronous action and handles any exceptions that occur.
        /// </summary>
        /// <param name="action">The asynchronous action to execute</param>
        /// <param name="context">Context information about where the action is being executed</param>
        /// <param name="showToUser">Whether to show a message to the user if an exception occurs</param>
        /// <param name="parentWindow">The parent window for the message dialog</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public static async Task ExecuteAsync(Func<Task> action, string context, bool showToUser = true, Window? parentWindow = null)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, context, showToUser, parentWindow);
            }
        }

        /// <summary>
        /// Executes an asynchronous function and handles any exceptions that occur.
        /// </summary>
        /// <typeparam name="T">The return type of the function</typeparam>
        /// <param name="func">The asynchronous function to execute</param>
        /// <param name="context">Context information about where the function is being executed</param>
        /// <param name="defaultValue">The default value to return if an exception occurs</param>
        /// <param name="showToUser">Whether to show a message to the user if an exception occurs</param>
        /// <param name="parentWindow">The parent window for the message dialog</param>
        /// <returns>A task representing the asynchronous operation that returns the result of the function or the default value if an exception occurs</returns>
        public static async Task<T> ExecuteAsync<T>(Func<Task<T>> func, string context, T defaultValue, bool showToUser = true, Window? parentWindow = null)
        {
            try
            {
                return await func();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, context, showToUser, parentWindow);
                return defaultValue;
            }
        }

        /// <summary>
        /// Shows an error message to the user.
        /// </summary>
        /// <param name="message">The error message to show</param>
        /// <param name="parentWindow">The parent window for the message dialog</param>
        /// <returns>A task representing the asynchronous operation</returns>
        private static async Task ShowErrorMessageAsync(string message, Window? parentWindow)
        {
            try
            {
                var dialog = parentWindow != null
                    ? new ErrorDialog(message, parentWindow)
                    : new ErrorDialog(message);

                if (parentWindow != null)
                {
                    await dialog.ShowDialog(parentWindow);
                }
                else
                {
                    dialog.Show();
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