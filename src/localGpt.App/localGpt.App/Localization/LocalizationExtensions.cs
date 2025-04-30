using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using localGpt.App.Logging;
using System;

namespace localGpt.App.Localization
{
    /// <summary>
    /// Provides extension methods and markup extensions for localization.
    /// </summary>
    public static class LocalizationExtensions
    {
        /// <summary>
        /// Gets a localized string by key.
        /// </summary>
        /// <param name="key">Resource key</param>
        /// <returns>Localized string</returns>
        public static string Localize(this string key)
        {
            try
            {
                Logger.Debug("Localizing key: {Key}", key);
                return LocalizationManager.Instance.GetString(key);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error localizing key {Key}: {Message}", key, ex.Message);
                return key;
            }
        }
    }

    /// <summary>
    /// Markup extension for localization in XAML.
    /// </summary>
    public class LocalizeExtension : MarkupExtension
    {
        /// <summary>
        /// Gets or sets the resource key.
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// Provides the value for the markup extension.
        /// </summary>
        /// <param name="serviceProvider">Service provider</param>
        /// <returns>Localized string</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            try
            {
                Logger.Debug("Creating localization binding for key: {Key}", Key);

                // Create a binding that updates when the language changes
                return new Binding
                {
                    Source = LocalizationManager.Instance,
                    Path = "CurrentLanguage", // This property doesn't matter, we just need something to trigger the binding
                    Converter = new LocalizationConverter(Key)
                };
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error providing value for localization key {Key}: {Message}", Key, ex.Message);
                return new Binding();
            }
        }
    }

    /// <summary>
    /// Converter for localization binding.
    /// </summary>
    public class LocalizationConverter : IValueConverter
    {
        private readonly string _key;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizationConverter"/> class.
        /// </summary>
        /// <param name="key">Resource key</param>
        public LocalizationConverter(string key)
        {
            _key = key;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="targetType">Target type</param>
        /// <param name="parameter">Converter parameter</param>
        /// <param name="culture">Culture</param>
        /// <returns>Converted value</returns>
        public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                Logger.Debug("Converting localization for key: {Key}", _key);
                return LocalizationManager.Instance.GetString(_key);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error converting localization for key {Key}: {Message}", _key, ex.Message);
                return _key;
            }
        }

        /// <summary>
        /// Converts a value back.
        /// </summary>
        /// <param name="value">Value to convert back</param>
        /// <param name="targetType">Target type</param>
        /// <param name="parameter">Converter parameter</param>
        /// <param name="culture">Culture</param>
        /// <returns>Converted value</returns>
        public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                Logger.Warning("ConvertBack called on LocalizationConverter which is not implemented");
                throw new NotImplementedException("LocalizationConverter.ConvertBack is not implemented");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error in ConvertBack: {Message}", ex.Message);
                return null;
            }
        }
    }
}