using System.Text.Json.Serialization;

namespace localGptLib
{
    public class localGptLibUserDraft
    {
        [JsonPropertyName ("type")]
        [JsonConverter (typeof (localGptLibContentTypeJsonConverter))]
        public localGptLibContentType Type { get; set; }

        public string Prompt { get; set; }
    }
}
