using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using localGpt.App.Logging;
using System;
using System.ComponentModel;

namespace localGpt.App.Localization
{
    /// <summary>
    /// Dialog for selecting a language.
    /// </summary>
    /// <summary>
    /// This class is obsolete. Use <see cref="Views.LanguageSelectionDialog"/> instead.
    /// </summary>
    [Obsolete("This class is obsolete. Use Views.LanguageSelectionDialog instead.")]
    public class LanguageSelectionDialog : Window
    {
        private ComboBox _languageComboBox = new ComboBox
        {
            Width = 200,
            Margin = new Thickness(10)
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageSelectionDialog"/> class.
        /// </summary>
        /// <param name="owner">The owner window</param>
        public LanguageSelectionDialog(Window owner)
        {
            try
            {
                Logger.Information("Creating language selection dialog");

                // Set window properties
                Title = LocalizationManager.Instance.GetString("dialog.language.title");
                Width = 300;
                Height = 150;
                WindowStartupLocation = WindowStartupLocation.CenterOwner;

                // Configure the combo box for language selection

                // Add available languages to the combo box
                foreach (var language in LocalizationManager.Instance.AvailableLanguages)
                {
                    Logger.Debug("Adding language to selection: {Language}", language);
                    _languageComboBox.Items.Add(language);
                }

                // Set the selected language
                _languageComboBox.SelectedItem = LocalizationManager.Instance.CurrentLanguage;
                Logger.Debug("Current language selected: {Language}", LocalizationManager.Instance.CurrentLanguage);

                // Create a button to apply the selected language
                var applyButton = new Button
                {
                    Content = "OK",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(10)
                };

                // Handle the button click event
                applyButton.Click += OnApplyButtonClick;

                // Create a stack panel to hold the controls
                var stackPanel = new StackPanel
                {
                    Margin = new Thickness(10)
                };

                // Add a label for the combo box
                stackPanel.Children.Add(new TextBlock
                {
                    Text = LocalizationManager.Instance.GetString("dialog.language.selectLanguage"),
                    Margin = new Thickness(0, 0, 0, 5)
                });

                // Add the combo box and button to the stack panel
                stackPanel.Children.Add(_languageComboBox);
                stackPanel.Children.Add(applyButton);

                // Set the content of the window
                Content = stackPanel;

                Logger.Information("Language selection dialog created");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error creating language selection dialog: {Message}", ex.Message);
                Application.Current.ShowErrorDialog(ex.Message, owner);
            }
        }

        /// <summary>
        /// Handles the apply button click event.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void OnApplyButtonClick(object? sender, EventArgs e)
        {
            try
            {
                // Set the selected language
                if (_languageComboBox.SelectedItem is string language)
                {
                    Logger.Information("Language selected: {Language}", language);
                    LocalizationManager.Instance.SetLanguage(language);
                }
                else
                {
                    Logger.Warning("No language selected");
                }

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