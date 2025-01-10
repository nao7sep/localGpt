using System.Text.Json.Serialization;

namespace localGptLib
{
    public class localGptLibConversation
    {
        [JsonPropertyName ("id")]
        public Guid Id { get; set; }

        public localGptLibMetadata? Metadata { get; set; }

        public string? SystemMessage { get; set; }

        public localGptLibModelSettings? ModelSettings { get; set; }

        public localGptLibLanguageSettings? TranslationSettings { get; set; }

        public IList <localGptLibExchange>? Exchanges { get; set; }

        public localGptLibCurrentExchange? CurrentExchange { get; set; }
    }
}
