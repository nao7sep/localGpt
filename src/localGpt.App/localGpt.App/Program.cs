using Avalonia;
using localGpt.App.Configuration;
using localGpt.App.Logging;
using System;
using System.Threading.Tasks;

namespace localGpt.App;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            // Initialize configuration as early as possible
            var config = ConfigurationManager.Instance;

            // Initialize logging using the configuration
            Logger.Initialize();
            Logger.Information("Application starting");

            // Set up global exception handling for the main thread
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                if (e.ExceptionObject is Exception exception)
                {
                    Logger.Fatal(exception, "Unhandled exception in main thread: {Message}", exception.Message);
                }
                else
                {
                    Logger.Fatal("Unhandled exception object in main thread: {Object}", e.ExceptionObject);
                }
            };

            // Set up global exception handling for tasks
            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                Logger.Fatal(e.Exception, "Unobserved task exception: {Message}", e.Exception.Message);
                e.SetObserved(); // Prevent the application from crashing
            };

            Logger.Information("Starting Avalonia application");
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            // Last resort exception handling
            try
            {
                Logger.Fatal(ex, "Fatal exception in Main: {Message}", ex.Message);
            }
            catch
            {
                // If logging fails, write to console as a last resort
                Console.Error.WriteLine($"Fatal exception in Main: {ex.Message}");
                Console.Error.WriteLine(ex.StackTrace);
            }

            // Re-throw to let the OS handle it
            throw;
        }
        finally
        {
            // Ensure logger is closed properly
            Logger.Close();
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace(); // Keep Avalonia's built-in logging for UI-specific issues
}
