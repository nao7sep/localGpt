using Avalonia;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Serilog;
using localGpt.ViewModels;
using localGpt.Services;
using Microsoft.Extensions.Options;
using localGpt.Models;
using Microsoft.Extensions.Logging; // Added for ILogger
using System.IO; // Added for Path

namespace localGpt;

sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        App.ServiceProvider = host.Services;

        // Log the log directory path
        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        var logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
        logger.LogInformation("Logging to directory: {LogDirectory}", Path.GetFullPath(logDirectory));
        // You could also construct the full expected log file path if needed:
        // Example: Serilog automatically appends the date (e.g., 'log-yyyyMMdd.ndjson')
        // var logFilePath = Path.Combine(logDirectory, $"log-{DateTime.Now:yyyyMMdd}.ndjson");
        // logger.LogInformation("Expected log file path for today: {LogFilePath}", Path.GetFullPath(logFilePath));

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                // Set base path to the application directory (optional but good practice)
                var basePath = AppContext.BaseDirectory;
                config.SetBasePath(basePath)
                      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .UseSerilog((context, services, configuration) => {
                // Configure Serilog entirely from appsettings.json and services
                configuration
                    .ReadFrom.Configuration(context.Configuration) // Reads sinks, levels, etc. from IConfiguration
                    .ReadFrom.Services(services)                   // Allows dependency injection into sinks, etc.
                    .Enrich.FromLogContext();
                    // Removed manual File sink configuration - rely on appsettings.json
            })
            .ConfigureServices((context, services) =>
            {
                // Register Configuration instance
                services.AddSingleton(context.Configuration);

                // Configure and register LocalizationOptions
                services.Configure<LocalizationOptions>(context.Configuration.GetSection("Localization"));

                // Register the LocalizationService
                services.AddSingleton<ILocalizationService, JsonLocalizationService>();

                // Register ViewModels
                services.AddTransient<MainWindowViewModel>();
                // Add other ViewModels and Services here

                // Removed commented-out direct culture setting and options registration
            });


    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            // Removed commented-out .LogToTrace()
            .UseSkia(); // Or other rendering backend

    // Removed commented-out nested helper class for LocalizationOptions
}
