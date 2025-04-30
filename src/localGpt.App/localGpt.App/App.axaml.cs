using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using localGpt.App.Localization;
using localGpt.App.Logging;
using localGpt.App.Settings;
using System;
using System.IO;
using System.Threading.Tasks;

namespace localGpt.App;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        // Initialize localization
        InitializeLocalization();
    }

    private async void InitializeLocalization()
    {
        try
        {
            // Get the localization directory
            string localizationDirectory = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Localization");

            // Initialize the localization manager
            await LocalizationManager.Instance.InitializeAsync(localizationDirectory);

            // Set the current language from settings
            LocalizationManager.Instance.SetLanguage(AppSettings.Instance.Language);

            // Subscribe to language changes to save the setting
            LocalizationManager.Instance.LanguageChanged += (sender, e) =>
            {
                AppSettings.Instance.Language = LocalizationManager.Instance.CurrentLanguage;
                Task.Run(() => AppSettings.Instance.SaveAsync());
            };
        }
        catch (Exception ex)
        {
            // In a real application, we would log this error
            Console.WriteLine($"Error initializing localization: {ex.Message}");
        }
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Initialize logging
        Logger.Initialize();
        Logger.Information("Application starting");

        // Set up global exception handling
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
            desktop.Exit += OnApplicationExit;

            // Handle exceptions in the UI thread
            Dispatcher.UIThread.UnhandledException += OnDispatcherUnhandledException;
        }

        base.OnFrameworkInitializationCompleted();
    }

    /// <summary>
    /// Handles unhandled exceptions in the application domain.
    /// </summary>
    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exception)
        {
            Logger.Fatal(exception, "Unhandled exception: {Message}", exception.Message);
        }
        else
        {
            Logger.Fatal("Unhandled exception object: {Object}", e.ExceptionObject);
        }
    }

    /// <summary>
    /// Handles unobserved task exceptions.
    /// </summary>
    private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        Logger.Fatal(e.Exception, "Unobserved task exception: {Message}", e.Exception.Message);
        e.SetObserved(); // Prevent the application from crashing
    }

    /// <summary>
    /// Handles unhandled exceptions in the UI thread.
    /// </summary>
    private void OnDispatcherUnhandledException(object? sender, DispatcherUnhandledExceptionEventArgs e)
    {
        Logger.Fatal(e.Exception, "Unhandled UI exception: {Message}", e.Exception.Message);
        e.Handled = true; // Prevent the application from crashing
    }

    /// <summary>
    /// Handles application exit.
    /// </summary>
    private void OnApplicationExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        Logger.Information("Application exiting");
        Logger.Close();
    }
}