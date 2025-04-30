using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace localGpt.App.Logging
{
    /// <summary>
    /// Provides logging functionality for the application.
    /// </summary>
    public static class Logger
    {
        private static bool _isInitialized;

        /// <summary>
        /// Initializes the logger.
        /// </summary>
        public static void Initialize()
        {
            if (_isInitialized)
                return;

            try
            {
                // Create logs directory if it doesn't exist
                var logsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                if (!Directory.Exists(logsDirectory))
                {
                    Directory.CreateDirectory(logsDirectory);
                }

                // Configure Serilog
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .Enrich.WithThreadId()
                    .Enrich.WithProcessId()
                    .Enrich.WithMachineName()
                    .Enrich.WithEnvironmentUserName()
                    .WriteTo.Debug()
                    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                    .WriteTo.File(
                        Path.Combine(logsDirectory, "log-.ndjson"),
                        rollingInterval: RollingInterval.Day,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                        formatProvider: null,
                        fileSizeLimitBytes: 10 * 1024 * 1024, // 10MB
                        retainedFileCountLimit: 31) // Keep logs for a month
                    .CreateLogger();

                _isInitialized = true;

                Information("Logging initialized");
            }
            catch (Exception ex)
            {
                // If we can't initialize logging, write to console as a last resort
                Console.Error.WriteLine($"Failed to initialize logging: {ex.Message}");
                Console.Error.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// Closes and flushes the logger.
        /// </summary>
        public static void Close()
        {
            if (!_isInitialized)
                return;

            try
            {
                Log.CloseAndFlush();
                _isInitialized = false;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to close logging: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="propertyValues">Property values</param>
        public static void Debug(string messageTemplate, params object[] propertyValues)
        {
            try
            {
                Log.Debug(messageTemplate, propertyValues);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to log debug message: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="propertyValues">Property values</param>
        public static void Information(string messageTemplate, params object[] propertyValues)
        {
            try
            {
                Log.Information(messageTemplate, propertyValues);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to log information message: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="propertyValues">Property values</param>
        public static void Warning(string messageTemplate, params object[] propertyValues)
        {
            try
            {
                Log.Warning(messageTemplate, propertyValues);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to log warning message: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="propertyValues">Property values</param>
        public static void Error(string messageTemplate, params object[] propertyValues)
        {
            try
            {
                Log.Error(messageTemplate, propertyValues);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to log error message: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs an error with exception details.
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="propertyValues">Property values</param>
        public static void Error(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            try
            {
                Log.Error(exception, messageTemplate, propertyValues);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to log error with exception: {ex.Message}");
                Console.Error.WriteLine($"Original exception: {exception.Message}");
            }
        }

        /// <summary>
        /// Logs a fatal error message.
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="propertyValues">Property values</param>
        public static void Fatal(string messageTemplate, params object[] propertyValues)
        {
            try
            {
                Log.Fatal(messageTemplate, propertyValues);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to log fatal message: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs a fatal error with exception details.
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="propertyValues">Property values</param>
        public static void Fatal(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            try
            {
                Log.Fatal(exception, messageTemplate, propertyValues);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to log fatal error with exception: {ex.Message}");
                Console.Error.WriteLine($"Original exception: {exception.Message}");
            }
        }
    }
}