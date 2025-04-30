using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using localGpt.App.Localization;
using localGpt.App.Localization.Views;
using localGpt.App.Logging;
using System;
using System.Linq;

namespace localGpt.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        try
        {
            Logger.Information("Initializing MainWindow");
            InitializeComponent();
            Logger.Information("MainWindow initialized");
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Error initializing MainWindow: {Message}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Handles the language settings menu item click event.
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event arguments</param>
    private async void OnLanguageSettingsClick(object sender, RoutedEventArgs e)
    {
        try
        {
            Logger.Information("Language settings menu item clicked");

            // Create and show the language selection dialog
            var dialog = new localGpt.App.Localization.Views.LanguageSelectionDialog(this);
            await dialog.ShowDialog(this);

            Logger.Information("Language selection dialog closed");
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Error showing language selection dialog: {Message}", ex.Message);
            Application.Current.ShowErrorDialog(ex.Message, this);
        }
    }
}