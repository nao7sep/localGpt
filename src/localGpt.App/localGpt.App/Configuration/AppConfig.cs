using System.Collections.Generic;

namespace localGpt.App.Configuration
{
    /// <summary>
    /// Represents the application configuration.
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// Gets or sets the application settings.
        /// </summary>
        public AppSettings AppSettings { get; set; } = new AppSettings();

        /// <summary>
        /// Gets or sets the logging settings.
        /// </summary>
        public LoggingSettings Logging { get; set; } = new LoggingSettings();

        /// <summary>
        /// Gets or sets the OpenAI API settings.
        /// </summary>
        public OpenAiSettings OpenAi { get; set; } = new OpenAiSettings();
    }

    /// <summary>
    /// Represents the application settings.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Gets or sets the default language.
        /// </summary>
        public string DefaultLanguage { get; set; } = "en-us";

        /// <summary>
        /// Gets or sets the default model.
        /// </summary>
        public string DefaultModel { get; set; } = "gpt-4o";

        /// <summary>
        /// Gets or sets the default system prompt.
        /// </summary>
        public string DefaultSystemPrompt { get; set; } = "You are a helpful assistant.";

        /// <summary>
        /// Gets or sets the default session directory.
        /// </summary>
        public string DefaultSessionDirectory { get; set; } = "";
    }

    /// <summary>
    /// Represents the logging settings.
    /// </summary>
    public class LoggingSettings
    {
        /// <summary>
        /// Gets or sets the minimum log level.
        /// </summary>
        public string MinimumLevel { get; set; } = "Information";

        /// <summary>
        /// Gets or sets the log file path.
        /// </summary>
        public string LogFilePath { get; set; } = "logs/log-.ndjson";

        /// <summary>
        /// Gets or sets the maximum file size in bytes.
        /// </summary>
        public long FileSizeLimitBytes { get; set; } = 10 * 1024 * 1024; // 10MB

        /// <summary>
        /// Gets or sets the maximum number of log files to retain.
        /// </summary>
        public int RetainedFileCountLimit { get; set; } = 31; // Keep logs for a month
    }

    /// <summary>
    /// Represents the OpenAI API settings.
    /// </summary>
    public class OpenAiSettings
    {
        /// <summary>
        /// Gets or sets the API key.
        /// </summary>
        public string ApiKey { get; set; } = "";

        /// <summary>
        /// Gets or sets the organization ID.
        /// </summary>
        public string OrganizationId { get; set; } = "";

        /// <summary>
        /// Gets or sets the available models.
        /// </summary>
        public List<ModelInfo> Models { get; set; } = new List<ModelInfo>();
    }

    /// <summary>
    /// Represents information about an AI model.
    /// </summary>
    public class ModelInfo
    {
        /// <summary>
        /// Gets or sets the model name.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Gets or sets the prompt price per 1 million tokens.
        /// </summary>
        public double? PromptPricePer1M { get; set; }

        /// <summary>
        /// Gets or sets the completion price per 1 million tokens.
        /// </summary>
        public double? CompletionPricePer1M { get; set; }

        /// <summary>
        /// Gets a value indicating whether pricing is configured.
        /// </summary>
        public bool IsPricingConfigured => PromptPricePer1M.HasValue && CompletionPricePer1M.HasValue;
    }
}