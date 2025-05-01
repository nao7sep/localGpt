using localGpt.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace localGpt.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly ILogger<MainWindowViewModel> _logger;
    private readonly ILocalizationService _localizer;
    private string _selectedCulture;

    public MainWindowViewModel(ILogger<MainWindowViewModel> logger, ILocalizationService localizer)
    {
        _logger = logger;
        _localizer = localizer;
        _selectedCulture = _localizer.CurrentCultureName;
        _logger.LogInformation("MainWindowViewModel created.");

        _localizer.PropertyChanged += LocalizationService_PropertyChanged;
    }

    public ILocalizationService Localizer => _localizer;
    public IEnumerable<string> SupportedCultures => _localizer.SupportedCultures;

    public string SelectedCulture
    {
        get => _selectedCulture;
        set
        {
            if (SetProperty(ref _selectedCulture, value))
            {
                _localizer.SetCulture(value);
            }
        }
    }

    private void LocalizationService_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(ILocalizationService.CurrentCultureName))
        {
            if (_selectedCulture != _localizer.CurrentCultureName)
            {
                SelectedCulture = _localizer.CurrentCultureName;
            }

            OnPropertyChanged(nameof(Localizer));
        }
    }
}
