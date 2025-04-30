using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace localGpt.App.Localization
{
    /// <summary>
    /// Manages localization resources for the application.
    /// Loads strings from language JSON files and provides access to them.
    /// </summary>
    public class LocalizationManager
    {
        private static LocalizationManager? _instance;
        private readonly Dictionary<string, Dictionary<string, string>> _languageResources = new();
        private string _currentLanguage = "en-us"; // Default language

        // Singleton pattern
        public static LocalizationManager Instance => _instance ??= new LocalizationManager();

        // Private constructor for singleton
        private LocalizationManager()
        {
        }

        /// <summary>
        /// Initializes the localization manager by loading available language files.
        /// </summary>
        /// <param name="localizationDirectory">Directory containing language files</param>
        /// <returns>Async task</returns>
        public async Task InitializeAsync(string localizationDirectory)
        {
            // Ensure the directory exists
            if (!Directory.Exists(localizationDirectory))
            {
                Directory.CreateDirectory(localizationDirectory);
            }

            // Load all language files
            foreach (var file in Directory.GetFiles(localizationDirectory, "*.json"))
            {
                var languageCode = Path.GetFileNameWithoutExtension(file);
                await LoadLanguageFileAsync(languageCode, file);
            }

            // If no languages were loaded, create default ones
            if (_languageResources.Count == 0)
            {
                await CreateDefaultLanguageFilesAsync(localizationDirectory);
            }
        }

        /// <summary>
        /// Creates default language files if none exist.
        /// </summary>
        /// <param name="localizationDirectory">Directory to create files in</param>
        /// <returns>Async task</returns>
        private async Task CreateDefaultLanguageFilesAsync(string localizationDirectory)
        {
            // Create en-us.json
            var enUsPath = Path.Combine(localizationDirectory, "en-us.json");
            var enUsStrings = GetDefaultEnglishStrings();
            await File.WriteAllTextAsync(enUsPath, JsonSerializer.Serialize(enUsStrings, new JsonSerializerOptions { WriteIndented = true }));
            _languageResources["en-us"] = enUsStrings;

            // Create ja-jp.json
            var jaJpPath = Path.Combine(localizationDirectory, "ja-jp.json");
            var jaJpStrings = GetDefaultJapaneseStrings();
            await File.WriteAllTextAsync(jaJpPath, JsonSerializer.Serialize(jaJpStrings, new JsonSerializerOptions { WriteIndented = true }));
            _languageResources["ja-jp"] = jaJpStrings;
        }

        /// <summary>
        /// Loads a language file into memory.
        /// </summary>
        /// <param name="languageCode">Language code (e.g., "en-us")</param>
        /// <param name="filePath">Path to the language file</param>
        /// <returns>Async task</returns>
        private async Task LoadLanguageFileAsync(string languageCode, string filePath)
        {
            try
            {
                var json = await File.ReadAllTextAsync(filePath);
                var resources = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                if (resources != null)
                {
                    _languageResources[languageCode] = resources;
                }
            }
            catch (Exception ex)
            {
                // In a real application, we would log this error
                Console.WriteLine($"Error loading language file {filePath}: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets a localized string by key.
        /// </summary>
        /// <param name="key">Resource key</param>
        /// <returns>Localized string or key if not found</returns>
        public string GetString(string key)
        {
            // Try to get the string from the current language
            if (_languageResources.TryGetValue(_currentLanguage, out var resources) &&
                resources.TryGetValue(key, out var value))
            {
                return value;
            }

            // Fall back to English if the string is not found in the current language
            if (_currentLanguage != "en-us" &&
                _languageResources.TryGetValue("en-us", out var enResources) &&
                enResources.TryGetValue(key, out var enValue))
            {
                return enValue;
            }

            // Return the key if the string is not found in any language
            return key;
        }

        /// <summary>
        /// Sets the current language.
        /// </summary>
        /// <param name="languageCode">Language code (e.g., "en-us")</param>
        /// <returns>True if the language was set successfully, false otherwise</returns>
        public bool SetLanguage(string languageCode)
        {
            if (_languageResources.ContainsKey(languageCode))
            {
                _currentLanguage = languageCode;
                // Raise an event to notify the UI that the language has changed
                LanguageChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the current language code.
        /// </summary>
        public string CurrentLanguage => _currentLanguage;

        /// <summary>
        /// Gets a list of available languages.
        /// </summary>
        public IEnumerable<string> AvailableLanguages => _languageResources.Keys;

        /// <summary>
        /// Event raised when the language is changed.
        /// </summary>
        public event EventHandler? LanguageChanged;

        /// <summary>
        /// Gets the default English strings.
        /// </summary>
        /// <returns>Dictionary of English strings</returns>
        private Dictionary<string, string> GetDefaultEnglishStrings()
        {
            return new Dictionary<string, string>
            {
                { "app.title", "localGpt" },
                { "menu.file", "File" },
                { "menu.edit", "Edit" },
                { "menu.view", "View" },
                { "menu.tools", "Tools" },
                { "menu.settings", "Settings" },
                { "menu.help", "Help" },
                { "menu.file.new", "New Window" },
                { "menu.file.open", "Open" },
                { "menu.file.save", "Save" },
                { "menu.file.saveAs", "Save As" },
                { "menu.file.exit", "Exit" },
                { "menu.edit.copy", "Copy" },
                { "menu.edit.paste", "Paste" },
                { "menu.edit.delete", "Delete" },
                { "menu.edit.regenerate", "Regenerate" },
                { "menu.view.toggleModelPane", "Toggle Model Pane" },
                { "menu.view.toggleParameterPane", "Toggle Parameter Pane" },
                { "menu.view.toggleMetaInfo", "Toggle Meta Info" },
                { "menu.tools.search", "Full-text Search" },
                { "menu.tools.tokenPriceChart", "Token-price Quick Chart" },
                { "menu.tools.htmlExport", "HTML Export" },
                { "menu.settings.modelManager", "Model Manager" },
                { "menu.settings.language", "Language" },
                { "menu.help.about", "About" },
                { "menu.help.shortcuts", "Shortcuts" },
                { "menu.help.openConfigFolder", "Open Config Folder" },
                { "button.send", "Send" },
                { "label.search", "Search" },
                { "status.totalPromptTokens", "Total Prompt Tokens" },
                { "status.totalCompletionTokens", "Total Completion Tokens" },
                { "status.promptCost", "Prompt Cost" },
                { "status.completionCost", "Completion Cost" },
                { "status.totalCost", "Total Cost" },
                { "dialog.language.title", "Language Settings" },
                { "dialog.language.selectLanguage", "Select Language" },
                { "dialog.modelManager.title", "Model Manager" },
                { "dialog.modelManager.name", "Name" },
                { "dialog.modelManager.promptPrice", "Prompt Price (per 1M)" },
                { "dialog.modelManager.completionPrice", "Completion Price (per 1M)" },
                { "dialog.modelManager.add", "Add" },
                { "dialog.modelManager.edit", "Edit" },
                { "dialog.modelManager.delete", "Delete" },
                { "dialog.saveAs.title", "Save As" },
                { "dialog.saveAs.filename", "Filename" },
                { "dialog.saveAs.save", "Save" },
                { "dialog.saveAs.cancel", "Cancel" }
            };
        }

        /// <summary>
        /// Gets the default Japanese strings.
        /// </summary>
        /// <returns>Dictionary of Japanese strings</returns>
        private Dictionary<string, string> GetDefaultJapaneseStrings()
        {
            return new Dictionary<string, string>
            {
                { "app.title", "ローカルGPT" },
                { "menu.file", "ファイル" },
                { "menu.edit", "編集" },
                { "menu.view", "表示" },
                { "menu.tools", "ツール" },
                { "menu.settings", "設定" },
                { "menu.help", "ヘルプ" },
                { "menu.file.new", "新規ウィンドウ" },
                { "menu.file.open", "開く" },
                { "menu.file.save", "保存" },
                { "menu.file.saveAs", "名前を付けて保存" },
                { "menu.file.exit", "終了" },
                { "menu.edit.copy", "コピー" },
                { "menu.edit.paste", "貼り付け" },
                { "menu.edit.delete", "削除" },
                { "menu.edit.regenerate", "再生成" },
                { "menu.view.toggleModelPane", "モデルペインの切り替え" },
                { "menu.view.toggleParameterPane", "パラメータペインの切り替え" },
                { "menu.view.toggleMetaInfo", "メタ情報の切り替え" },
                { "menu.tools.search", "全文検索" },
                { "menu.tools.tokenPriceChart", "トークン価格チャート" },
                { "menu.tools.htmlExport", "HTML出力" },
                { "menu.settings.modelManager", "モデル管理" },
                { "menu.settings.language", "言語" },
                { "menu.help.about", "バージョン情報" },
                { "menu.help.shortcuts", "ショートカット" },
                { "menu.help.openConfigFolder", "設定フォルダを開く" },
                { "button.send", "送信" },
                { "label.search", "検索" },
                { "status.totalPromptTokens", "合計プロンプトトークン" },
                { "status.totalCompletionTokens", "合計完了トークン" },
                { "status.promptCost", "プロンプトコスト" },
                { "status.completionCost", "完了コスト" },
                { "status.totalCost", "合計コスト" },
                { "dialog.language.title", "言語設定" },
                { "dialog.language.selectLanguage", "言語を選択" },
                { "dialog.modelManager.title", "モデル管理" },
                { "dialog.modelManager.name", "名前" },
                { "dialog.modelManager.promptPrice", "プロンプト価格（100万あたり）" },
                { "dialog.modelManager.completionPrice", "完了価格（100万あたり）" },
                { "dialog.modelManager.add", "追加" },
                { "dialog.modelManager.edit", "編集" },
                { "dialog.modelManager.delete", "削除" },
                { "dialog.saveAs.title", "名前を付けて保存" },
                { "dialog.saveAs.filename", "ファイル名" },
                { "dialog.saveAs.save", "保存" },
                { "dialog.saveAs.cancel", "キャンセル" }
            };
        }
    }
}