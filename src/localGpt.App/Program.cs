using Avalonia;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Serilog;
using localGpt.ViewModels; // Assuming MainWindowViewModel is here
using System.IO;
using System.Globalization;
using Microsoft.Extensions.Localization;

namespace localGpt;

sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        // Store the service provider for later use in App.axaml.cs
        App.ServiceProvider = host.Services;

        // Configure Avalonia App
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                // Set base path to the application directory
                var basePath = AppContext.BaseDirectory;
                config.SetBasePath(basePath)
                      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext())
            .ConfigureServices((context, services) =>
            {
                // Register Configuration instance
                services.AddSingleton(context.Configuration);

                // Configure Localization
                services.AddLocalization(options => options.ResourcesPath = "localization");

                // Register ViewModels (as Transient or Singleton depending on need)
                services.AddTransient<MainWindowViewModel>();
                // Add other ViewModels and Services here

                // Example: Configure supported cultures from appsettings.json
                var localizationOptions = context.Configuration.GetSection("Localization").Get<LocalizationOptions>()
                                          ?? new LocalizationOptions { DefaultCulture = "en-US", SupportedCultures = ["en-US", "ja-JP"] };

                CultureInfo.CurrentCulture = new CultureInfo(localizationOptions.DefaultCulture);
                CultureInfo.CurrentUICulture = new CultureInfo(localizationOptions.DefaultCulture);

                // You might want a service to manage culture switching later
                services.AddSingleton(localizationOptions);
            });


    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            // Remove Avalonia's default LogToTrace if using Serilog for everything
            // .LogToTrace();
            .UseSkia(); // Or other rendering backend

    // Helper class for Localization options binding
    public class LocalizationOptions
    {
        public string DefaultCulture { get; set; } = "en-US";
        public string[] SupportedCultures { get; set; } = ["en-US", "ja-JP"];
    }
}
