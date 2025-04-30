using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using localGpt.App.Localization;
using localGpt.App.Logging;
using System.Linq;

namespace localGpt.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        ExceptionHandler.Execute(() =>
        {
            Logger.Information("Initializing MainWindow");
            InitializeComponent();
            Logger.Information("MainWindow initialized");
        }, "MainWindow.Constructor");
    }

    /// <summary>
    /// Handles the language settings menu item click event.
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event arguments</param>
    private async void OnLanguageSettingsClick(object sender, RoutedEventArgs e)
    {
        await ExceptionHandler.ExecuteAsync(async () =>
        {
            Logger.Information("Language settings menu item clicked");

            // Create and show the language selection dialog
            var dialog = new LanguageSelectionDialog(this);
            await dialog.ShowDialog(this);

            Logger.Information("Language selection dialog closed");
        }, "MainWindow.OnLanguageSettingsClick", showToUser: true, parentWindow: this);
    }
}