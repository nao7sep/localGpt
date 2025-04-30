using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using localGpt.App.Localization;
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
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}