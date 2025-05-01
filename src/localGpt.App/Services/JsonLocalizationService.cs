using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using localGpt.Models; // Use the new Models namespace
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options; // Required for IOptions
using System.Linq; // Add this for LINQ extension methods like Contains

namespace localGpt.Services;

/// <summary>
/// Provides localization services by loading strings from JSON files.
/// </summary>
public class JsonLocalizationService : ILocalizationService
{
    private readonly ILogger<JsonLocalizationService> _logger;
    private readonly string _resourcesPath;
    private Dictionary<string, string>? _localizedStrings;
    private CultureInfo _currentCulture;
    private readonly LocalizationOptions _localizationOptions; // Use the top-level class

    public event PropertyChangedEventHandler? PropertyChanged;

    // Inject IOptions<LocalizationOptions> using the top-level class
    public JsonLocalizationService(ILogger<JsonLocalizationService> logger, IOptions<LocalizationOptions> localizationOptions)
    {
        _logger = logger;
        // Get options value
        _localizationOptions = localizationOptions.Value ?? throw new ArgumentNullException(nameof(localizationOptions), "LocalizationOptions cannot be null.");

        // Construct the absolute path to the localization directory
        _resourcesPath = Path.Combine(AppContext.BaseDirectory, "localization");
        _logger.LogInformation("Localization resources path set to: {Path}", _resourcesPath);

        // Set initial culture from options
        _currentCulture = new CultureInfo(_localizationOptions.DefaultCulture);
        LoadLocalizedStrings();
    }

    public string CurrentCultureName => _currentCulture.Name;

    /// <summary>
    /// Loads the localization strings for the current culture.
    /// </summary>
    private void LoadLocalizedStrings()
    {
        var cultureName = _currentCulture.Name;
        var filePath = Path.Combine(_resourcesPath, $"{cultureName}.json");
        _logger.LogInformation("Attempting to load localization file: {FilePath}", filePath);

        if (!File.Exists(filePath))
        {
            _logger.LogWarning("Localization file not found: {FilePath}. Falling back to default or empty.", filePath);
            // Optionally, try loading the default culture file if the current one isn't found
            if (cultureName != _localizationOptions.DefaultCulture)
            {
                var defaultFilePath = Path.Combine(_resourcesPath, $"{_localizationOptions.DefaultCulture}.json");
                if (File.Exists(defaultFilePath))
                {
                    _logger.LogInformation("Falling back to default localization file: {DefaultFilePath}", defaultFilePath);
                    filePath = defaultFilePath;
                }
                else
                {
                     _logger.LogWarning("Default localization file also not found: {DefaultFilePath}. Using empty dictionary.", defaultFilePath);
                    _localizedStrings = new Dictionary<string, string>();
                    return; // Exit if neither file exists
                }
            }
            else // If default is missing
            {
                 _logger.LogWarning("Default localization file not found: {DefaultFilePath}. Using empty dictionary.", filePath);
                _localizedStrings = new Dictionary<string, string>();
                return; // Exit if default file doesn't exist
            }
        }

        try
        {
            var json = File.ReadAllText(filePath);
            _localizedStrings = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            _logger.LogInformation("Successfully loaded {Count} strings from {FilePath}", _localizedStrings?.Count ?? 0, filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load or parse localization file: {FilePath}", filePath);
            _localizedStrings = new Dictionary<string, string>(); // Use empty dictionary on error
        }
    }

    /// <summary>
    /// Gets the localized string for the specified key.
    /// </summary>
    public string GetString(string key)
    {
        if (_localizedStrings != null && _localizedStrings.TryGetValue(key, out var value))
        {
            return value;
        }

        _logger.LogWarning("Localization key not found: '{Key}' for culture {CultureName}", key, _currentCulture.Name);
        return key; // Return the key itself as a fallback
    }

    /// <summary>
    /// Indexer for easy access: Localizer["key"]
    /// </summary>
    public string this[string key] => GetString(key);

    /// <summary>
    /// Sets the current culture and reloads the strings.
    /// </summary>
    public void SetCulture(string cultureName)
    {
        if (!(_localizationOptions.SupportedCultures?.Contains(cultureName) ?? false))
        {
             _logger.LogWarning("Attempted to set unsupported culture: {CultureName}. Supported are: {SupportedCultures}",
                 cultureName, string.Join(", ", _localizationOptions.SupportedCultures ?? ["N/A"]));
            return; // Or throw an exception, depending on desired behavior
        }

        var newCulture = new CultureInfo(cultureName);
        if (newCulture.Name != _currentCulture.Name)
        {
            _currentCulture = newCulture;
            _logger.LogInformation("Culture changed to: {CultureName}", _currentCulture.Name);
            LoadLocalizedStrings();
            // Notify that all properties might have changed due to the culture switch
            OnPropertyChanged(string.Empty); // Indicates all properties might have changed
            OnPropertyChanged(nameof(CurrentCultureName)); // Specifically notify for the culture name itself
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
