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
    /// <param name="key">The key of the string to retrieve.</param>
    /// <returns>The localized string, or the key itself if not found.</returns>
    string GetString(string key);

    /// <summary>
    /// Indexer to provide easy access to localized strings like Localizer["key"].
    /// </summary>
    string this[string key] { get; }

    /// <summary>
    /// Sets the current culture for localization.
    /// </summary>
    /// <param name="cultureName">The name of the culture (e.g., "en-US", "ja-JP").</param>
    void SetCulture(string cultureName);

    /// <summary>
    /// Gets the current culture name.
    /// </summary>
    string CurrentCultureName { get; }
}
