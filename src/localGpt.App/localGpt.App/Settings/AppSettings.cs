using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace localGpt.App.Settings
{
    /// <summary>
    /// Represents the application settings.
    /// </summary>
    public class AppSettings
    {
        private static readonly string SettingsDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "localGpt");

        private static readonly string SettingsFilePath = Path.Combine(
            SettingsDirectory,
            "settings.json");

        private static AppSettings? _instance;

        /// <summary>
        /// Gets the singleton instance of the application settings.
        /// </summary>
        public static AppSettings Instance => _instance ??= Load();

        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        public string Language { get; set; } = "en-us";

        /// <summary>
        /// Gets or sets the last opened directory.
        /// </summary>
        public string? LastOpenedDirectory { get; set; }

        /// <summary>
        /// Gets or sets the last selected model.
        /// </summary>
        public string? LastSelectedModel { get; set; }

        /// <summary>
        /// Loads the settings from the settings file.
        /// </summary>
        /// <returns>The loaded settings or default settings if the file doesn't exist.</returns>
        private static AppSettings Load()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    var json = File.ReadAllText(SettingsFilePath);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json);
                    return settings ?? new AppSettings();
                }
            }
            catch (Exception ex)
            {
                // In a real application, we would log this error
                Console.WriteLine($"Error loading settings: {ex.Message}");
            }

            return new AppSettings();
        }

        /// <summary>
        /// Saves the settings to the settings file.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SaveAsync()
        {
            try
            {
                // Ensure the directory exists
                if (!Directory.Exists(SettingsDirectory))
                {
                    Directory.CreateDirectory(SettingsDirectory);
                }

                var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(SettingsFilePath, json);
            }
            catch (Exception ex)
            {
                // In a real application, we would log this error
                Console.WriteLine($"Error saving settings: {ex.Message}");
            }
        }
    }
}