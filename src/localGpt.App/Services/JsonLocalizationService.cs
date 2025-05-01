using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using localGpt.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;

namespace localGpt.Services;

public class JsonLocalizationService : ILocalizationService
{
    private readonly ILogger<JsonLocalizationService> _logger;
    private readonly string _resourcesPath;
    private Dictionary<string, string> _localizedStrings = new();
    private CultureInfo _currentCulture;
    private readonly LocalizationOptions _localizationOptions;

    public event PropertyChangedEventHandler? PropertyChanged;

    public JsonLocalizationService(ILogger<JsonLocalizationService> logger, IOptions<LocalizationOptions> localizationOptions)
    {
        _logger = logger;
        _localizationOptions = localizationOptions.Value ?? throw new ArgumentNullException(nameof(localizationOptions), "LocalizationOptions cannot be null.");
        _resourcesPath = Path.Combine(AppContext.BaseDirectory, "localization");
        _logger.LogInformation("Localization resources path set to: {Path}", _resourcesPath);
        _currentCulture = new CultureInfo(_localizationOptions.DefaultCulture);
        LoadLocalizedStrings();
    }

    public string CurrentCultureName => _currentCulture.Name;
    public IEnumerable<string> SupportedCultures => _localizationOptions.SupportedCultures ?? Enumerable.Empty<string>();
    public string this[string key] => GetString(key);

    private void LoadLocalizedStrings()
    {
        var cultureName = _currentCulture.Name;
        var filePath = Path.Combine(_resourcesPath, $"{cultureName}.json");
        _logger.LogInformation("Attempting to load localization file: {FilePath}", filePath);

        if (!File.Exists(filePath))
        {
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
                    _logger.LogWarning("Neither requested nor default localization files found. Using empty dictionary.");
                    _localizedStrings = new Dictionary<string, string>();
                    return;
                }
            }
            else
            {
                _logger.LogWarning("Default localization file not found. Using empty dictionary.");
                _localizedStrings = new Dictionary<string, string>();
                return;
            }
        }

        try
        {
            var json = File.ReadAllText(filePath);
            var strings = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            _localizedStrings = strings ?? new Dictionary<string, string>();
            _logger.LogInformation("Successfully loaded {Count} strings from {FilePath}", _localizedStrings.Count, filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load or parse localization file: {FilePath}", filePath);
            _localizedStrings = new Dictionary<string, string>();
        }
    }

    public string GetString(string key)
    {
        if (_localizedStrings.TryGetValue(key, out var value))
        {
            return value;
        }

        _logger.LogWarning("Localization key not found: '{Key}' for culture {CultureName}", key, _currentCulture.Name);
        return key;
    }

    public void SetCulture(string cultureName)
    {
        if (!(_localizationOptions.SupportedCultures?.Contains(cultureName) ?? false))
        {
            _logger.LogWarning("Attempted to set unsupported culture: {CultureName}. Supported: {SupportedCultures}",
                cultureName, string.Join(", ", _localizationOptions.SupportedCultures ?? ["N/A"]));
            return;
        }

        var newCulture = new CultureInfo(cultureName);
        if (newCulture.Name != _currentCulture.Name)
        {
            _currentCulture = newCulture;
            _logger.LogInformation("Culture changed to: {CultureName}", _currentCulture.Name);
            LoadLocalizedStrings();
            OnPropertyChanged(string.Empty);
            OnPropertyChanged(nameof(CurrentCultureName));
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
