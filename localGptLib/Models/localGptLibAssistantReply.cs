using System.Text.Json.Serialization;

namespace localGptLib
{
    public class localGptLibAssistantReply
    {
        [JsonPropertyName ("type")]
        [JsonConverter (typeof (localGptLibContentTypeJsonConverter))]
        public localGptLibContentType Type { get; set; }

        public string Content { get; set; }
        public string ImageFileName { get; set; }
        public string RevisedPrompt { get; set; }

        public string? Title { get; set; }

        public string? Summary { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public IList <localGptLibTranslation>? Translations { get; set; }
    }
}
