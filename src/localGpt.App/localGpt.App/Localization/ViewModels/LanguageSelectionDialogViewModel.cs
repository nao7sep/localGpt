using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using localGpt.App.Logging;

namespace localGpt.App.Localization.ViewModels
{
    /// <summary>
    /// ViewModel for the language selection dialog.
    /// </summary>
    public class LanguageSelectionDialogViewModel : INotifyPropertyChanged
    {
        private string _selectedLanguage;

        /// <summary>
        /// Gets the available languages.
        /// </summary>
        public ObservableCollection<string> AvailableLanguages { get; } = new ObservableCollection<string>();

        /// <summary>
        /// Gets or sets the selected language.
        /// </summary>
        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                if (_selectedLanguage != value)
                {
                    _selectedLanguage = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageSelectionDialogViewModel"/> class.
        /// </summary>
        public LanguageSelectionDialogViewModel()
        {
            try
            {
                Logger.Information("Creating language selection dialog view model");

                // Load available languages
                foreach (var language in LocalizationManager.Instance.AvailableLanguages)
                {
                    Logger.Debug("Adding language to selection: {Language}", language);
                    AvailableLanguages.Add(language);
                }

                // Set the selected language
                SelectedLanguage = LocalizationManager.Instance.CurrentLanguage;
                Logger.Debug("Current language selected: {Language}", SelectedLanguage);

                Logger.Information("Language selection dialog view model created");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error creating language selection dialog view model: {Message}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Applies the selected language.
        /// </summary>
        public void ApplyLanguage()
        {
            try
            {
                // Set the selected language
                if (!string.IsNullOrEmpty(SelectedLanguage))
                {
                    Logger.Information("Language selected: {Language}", SelectedLanguage);
                    LocalizationManager.Instance.SetLanguage(SelectedLanguage);
                }
                else
                {
                    Logger.Warning("No language selected");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error applying language: {Message}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Event raised when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}