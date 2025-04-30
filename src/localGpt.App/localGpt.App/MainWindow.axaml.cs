using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using localGpt.App.Localization;
using System.Linq;

namespace localGpt.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Handles the language settings menu item click event.
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event arguments</param>
    private async void OnLanguageSettingsClick(object sender, RoutedEventArgs e)
    {
        // Create and show the language selection dialog
        var dialog = new LanguageSelectionDialog(this);
        await dialog.ShowDialog(this);
    }
}