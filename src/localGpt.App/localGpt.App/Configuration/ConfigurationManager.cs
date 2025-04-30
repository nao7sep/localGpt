using localGpt.App.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace localGpt.App.Configuration
{
    /// <summary>
    /// Manages application configuration.
    /// </summary>
    public class ConfigurationManager
    {
        private static ConfigurationManager? _instance;
        private readonly IConfiguration _configuration;
        private AppConfig? _appConfig;

        /// <summary>
        /// Gets the singleton instance of the configuration manager.
        /// </summary>
        public static ConfigurationManager Instance => _instance ??= new ConfigurationManager();

        /// <summary>
        /// Gets the application configuration.
        /// </summary>
        public AppConfig AppConfig => _appConfig ??= LoadAppConfig();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationManager"/> class.
        /// </summary>
        private ConfigurationManager()
        {
            try
            {
                // Build configuration
                var builder = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();

                _configuration = builder.Build();

                Logger.Information("Configuration initialized");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error initializing configuration");
                throw;
            }
        }

        /// <summary>
        /// Loads the application configuration from the configuration sources.
        /// </summary>
        /// <returns>The application configuration</returns>
        private AppConfig LoadAppConfig()
        {
            try
            {
                Logger.Information("Loading application configuration");

                var config = new AppConfig();
                _configuration.Bind(config);

                // If no models are configured, add default models
                if (config.OpenAi.Models.Count == 0)
                {
                    Logger.Information("No models configured, adding defaults");
                    config.OpenAi.Models.Add(new ModelInfo
                    {
                        Name = "gpt-4o",
                        PromptPricePer1M = 5.00,
                        CompletionPricePer1M = 15.00
                    });

                    config.OpenAi.Models.Add(new ModelInfo
                    {
                        Name = "gpt-4.1",
                        PromptPricePer1M = 2.00,
                        CompletionPricePer1M = 8.00
                    });
                }

                Logger.Information("Application configuration loaded successfully");
                return config;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error loading application configuration: {Message}", ex.Message);
                return new AppConfig();
            }
        }

        /// <summary>
        /// Saves the application configuration to the appsettings.json file.
        /// </summary>
        public void SaveAppConfig()
        {
            try
            {
                if (_appConfig == null)
                {
                    Logger.Warning("Cannot save null configuration");
                    return;
                }

                Logger.Information("Saving application configuration");

                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                var json = System.Text.Json.JsonSerializer.Serialize(_appConfig, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(filePath, json);

                Logger.Information("Application configuration saved successfully");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error saving application configuration: {Message}", ex.Message);
            }
        }

        /// <summary>
        /// Gets a configuration section.
        /// </summary>
        /// <param name="key">The configuration section key</param>
        /// <returns>The configuration section</returns>
        public IConfigurationSection GetSection(string key)
        {
            try
            {
                Logger.Debug("Getting configuration section: {Key}", key);
                return _configuration.GetSection(key);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error getting configuration section {Key}: {Message}", key, ex.Message);
                return _configuration.GetSection(string.Empty);
            }
        }

        /// <summary>
        /// Gets a configuration value.
        /// </summary>
        /// <typeparam name="T">The type of the configuration value</typeparam>
        /// <param name="key">The configuration key</param>
        /// <param name="defaultValue">The default value to return if the key is not found</param>
        /// <returns>The configuration value or the default value if the key is not found</returns>
        public T GetValue<T>(string key, T defaultValue)
        {
            try
            {
                Logger.Debug("Getting configuration value: {Key}", key);
                return _configuration.GetValue<T>(key, defaultValue);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error getting configuration value {Key}: {Message}", key, ex.Message);
                return defaultValue;
            }
        }
    }
}