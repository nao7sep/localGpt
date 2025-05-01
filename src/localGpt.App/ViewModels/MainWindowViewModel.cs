using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace localGpt.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ILogger<MainWindowViewModel> _logger;
    private readonly IStringLocalizer<MainWindowViewModel> _localizer;

    // Inject dependencies via constructor
    public MainWindowViewModel(ILogger<MainWindowViewModel> logger, IStringLocalizer<MainWindowViewModel> localizer)
    {
        _logger = logger;
        _localizer = localizer;
        _logger.LogInformation("MainWindowViewModel created.");
    }

    // Renamed property to reflect its purpose
    public string WindowTitle => _localizer["app.title"]; // Example: Use the app title key
}
