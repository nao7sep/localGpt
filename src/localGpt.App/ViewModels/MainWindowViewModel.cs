using localGpt.Services; // Replace Microsoft.Extensions.Localization
using Microsoft.Extensions.Logging;
using System.ComponentModel;

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
        _logger.LogInformation("MainWindowViewModel created.");

        // Subscribe to the localizer's PropertyChanged event
        _localizer.PropertyChanged += LocalizationService_PropertyChanged;
    }

    // Property to expose the localizer for binding (optional but good for dynamic updates)
    public ILocalizationService Localizer => _localizer;

    // Update the property to use the new service's indexer
    public string WindowTitle => _localizer["app.title"];

    private void LocalizationService_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // When the culture changes in the service (indicated by PropertyName being empty or null, or CurrentCultureName)
        // raise PropertyChanged for all properties that depend on localization.
        if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(ILocalizationService.CurrentCultureName))
        {
            OnPropertyChanged(nameof(WindowTitle));
            // Add other localized properties here
            OnPropertyChanged(nameof(Localizer)); // Notify that the indexer results might have changed
        }
    }

    // Consider adding a Dispose method or similar to unsubscribe from the event
    // if the ViewModel's lifetime is shorter than the singleton LocalizationService.
}
