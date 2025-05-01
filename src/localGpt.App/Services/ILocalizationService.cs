using System.Collections.Generic;
using System.ComponentModel;

namespace localGpt.Services;

/// <summary>
/// Defines the contract for a localization service.
/// </summary>
public interface ILocalizationService : INotifyPropertyChanged
{
    /// <summary>
    /// Gets the localized string for the specified key.
    /// </summary>
    string GetString(string key);

    /// <summary>
    /// Indexer for accessing localized strings: Localizer["key"]
    /// </summary>
    string this[string key] { get; }

    /// <summary>
    /// Sets the current culture for localization (e.g., "en-US", "ja-JP").
    /// </summary>
    void SetCulture(string cultureName);

    /// <summary>
    /// Gets the current culture name.
    /// </summary>
    string CurrentCultureName { get; }

    /// <summary>
    /// Gets the list of supported culture names.
    /// </summary>
    IEnumerable<string> SupportedCultures { get; }
}
