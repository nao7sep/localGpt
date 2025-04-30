using localGpt.App.Configuration;
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
                // Get logging settings from configuration
                var config = ConfigurationManager.Instance.AppConfig;
                var loggingSettings = config.Logging;

                // Parse minimum log level
                var minimumLevel = ParseLogLevel(loggingSettings.MinimumLevel);

                // Create logs directory if it doesn't exist
                var logFilePath = loggingSettings.LogFilePath;
                var logsDirectory = Path.GetDirectoryName(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logFilePath));
                if (!string.IsNullOrEmpty(logsDirectory) && !Directory.Exists(logsDirectory))
                {
                    Directory.CreateDirectory(logsDirectory);
                }

                // Configure Serilog
                var loggerConfiguration = new LoggerConfiguration()
                    .MinimumLevel.Is(minimumLevel)
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .Enrich.WithThreadId()
                    .Enrich.WithProcessId()
                    .Enrich.WithMachineName()
                    .Enrich.WithEnvironmentUserName()
                    .WriteTo.Debug()
                    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");

                // Add file logging if path is specified
                if (!string.IsNullOrEmpty(logFilePath))
                {
                    loggerConfiguration.WriteTo.File(
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logFilePath),
                        rollingInterval: RollingInterval.Day,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                        formatProvider: null,
                        fileSizeLimitBytes: loggingSettings.FileSizeLimitBytes,
                        retainedFileCountLimit: loggingSettings.RetainedFileCountLimit);
                }

                Log.Logger = loggerConfiguration.CreateLogger();

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

        /// <summary>
        /// Parses a log level string to a LogEventLevel.
        /// </summary>
        /// <param name="levelString">The log level string</param>
        /// <returns>The LogEventLevel</returns>
        private static LogEventLevel ParseLogLevel(string levelString)
        {
            return levelString?.ToLower() switch
            {
                "verbose" => LogEventLevel.Verbose,
                "debug" => LogEventLevel.Debug,
                "information" => LogEventLevel.Information,
                "warning" => LogEventLevel.Warning,
                "error" => LogEventLevel.Error,
                "fatal" => LogEventLevel.Fatal,
                _ => LogEventLevel.Information
            };
        }
    }
}