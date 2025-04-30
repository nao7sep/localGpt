using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using System;

namespace localGpt.App.Localization
{
    /// <summary>
    /// Dialog for selecting a language.
    /// </summary>
    public class LanguageSelectionDialog : Window
    {
        private readonly ComboBox _languageComboBox;

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageSelectionDialog"/> class.
        /// </summary>
        /// <param name="owner">The owner window</param>
        public LanguageSelectionDialog(Window owner)
        {
            // Set window properties
            Title = LocalizationManager.Instance.GetString("dialog.language.title");
            Width = 300;
            Height = 150;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            // Create a combo box for language selection
            _languageComboBox = new ComboBox
            {
                Width = 200,
                Margin = new Thickness(10)
            };

            // Add available languages to the combo box
            foreach (var language in LocalizationManager.Instance.AvailableLanguages)
            {
                _languageComboBox.Items.Add(language);
            }

            // Set the selected language
            _languageComboBox.SelectedItem = LocalizationManager.Instance.CurrentLanguage;

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
        }

        /// <summary>
        /// Handles the apply button click event.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void OnApplyButtonClick(object? sender, EventArgs e)
        {
            // Set the selected language
            if (_languageComboBox.SelectedItem is string language)
            {
                LocalizationManager.Instance.SetLanguage(language);
            }

            // Close the dialog
            Close();
        }
    }
}