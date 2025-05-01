using localGpt.Services; // Replace Microsoft.Extensions.Localization
using Microsoft.Extensions.Logging;
using System.Collections.Generic; // Add this
using System.ComponentModel;
using System.Linq; // Add this

namespace localGpt.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ILogger<MainWindowViewModel> _logger;
    // Change the type to the new interface
    private readonly ILocalizationService _localizer;

    // Update the constructor parameter type
    public MainWindowViewModel(ILogger<MainWindowViewModel> logger, ILocalizationService localizer)
    {
        _logger = logger;
        _localizer = localizer;
        _selectedCulture = _localizer.CurrentCultureName; // Initialize with current culture
        _logger.LogInformation("MainWindowViewModel created.");

        // Subscribe to the localizer's PropertyChanged event
        _localizer.PropertyChanged += LocalizationService_PropertyChanged;
    }

    // Property to expose the localizer for binding (optional but good for dynamic updates)
    public ILocalizationService Localizer => _localizer;

    // Property to expose supported cultures for the ComboBox
    // Ensure distinct values are returned
    public IEnumerable<string> SupportedCultures => _localizer.SupportedCultures.Distinct();

    // Property for the selected culture in the ComboBox
    private string _selectedCulture;
    public string SelectedCulture
    {
        get => _selectedCulture;
        set
        {
            if (SetProperty(ref _selectedCulture, value))
            {
                _localizer.SetCulture(value); // Change the culture when selection changes
            }
        }
    }

    // Update the property to use the new service's indexer
    public string WindowTitle => _localizer["app.title"];

    private void LocalizationService_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // When the culture changes in the service (indicated by PropertyName being empty or null, or CurrentCultureName)
        // raise PropertyChanged for all properties that depend on localization.
        if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(ILocalizationService.CurrentCultureName))
        {
            // Update SelectedCulture if it changed externally (e.g., initial load)
            if (_selectedCulture != _localizer.CurrentCultureName)
            {
                SelectedCulture = _localizer.CurrentCultureName;
            }

            OnPropertyChanged(nameof(WindowTitle));
            // Add other localized properties here
            OnPropertyChanged(nameof(Localizer)); // Notify that the indexer results might have changed
        }
    }

    // Consider adding a Dispose method or similar to unsubscribe from the event
    // if the ViewModel's lifetime is shorter than the singleton LocalizationService.
}
