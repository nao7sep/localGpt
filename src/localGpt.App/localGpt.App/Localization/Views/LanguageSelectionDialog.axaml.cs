using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using localGpt.App.Localization.ViewModels;
using localGpt.App.Logging;
using System;

namespace localGpt.App.Localization.Views
{
    /// <summary>
    /// Dialog for selecting a language.
    /// </summary>
    public partial class LanguageSelectionDialog : Window
    {
        private LanguageSelectionDialogViewModel ViewModel => DataContext as LanguageSelectionDialogViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageSelectionDialog"/> class.
        /// </summary>
        public LanguageSelectionDialog()
        {
            try
            {
                Logger.Information("Creating language selection dialog");
                InitializeComponent();
#if DEBUG
                this.AttachDevTools();
#endif
                DataContext = new LanguageSelectionDialogViewModel();
                Logger.Information("Language selection dialog created");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error creating language selection dialog: {Message}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageSelectionDialog"/> class.
        /// </summary>
        /// <param name="owner">The owner window</param>
        public LanguageSelectionDialog(Window owner) : this()
        {
            try
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error setting window startup location: {Message}", ex.Message);
                throw;
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Handles the apply button click event.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void OnApplyButtonClick(object? sender, RoutedEventArgs e)
        {
            try
            {
                // Apply the selected language
                ViewModel?.ApplyLanguage();

                // Close the dialog
                Logger.Debug("Closing language selection dialog");
                Close();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error applying language selection: {Message}", ex.Message);
                Application.Current.ShowErrorDialog(ex.Message, this);
            }
        }
    }
}