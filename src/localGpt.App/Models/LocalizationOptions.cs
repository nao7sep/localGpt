namespace localGpt.Models;

public class LocalizationOptions
{
    public string DefaultCulture { get; set; } = "en-US";
    public string[] SupportedCultures { get; set; } = ["en-US", "ja-JP"];
}
