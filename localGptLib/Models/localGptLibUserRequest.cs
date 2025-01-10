using System.Text.Json.Serialization;

namespace localGptLib
{
    public class localGptLibUserRequest
    {
        [JsonPropertyName ("type")]
        [JsonConverter (typeof (localGptLibContentTypeJsonConverter))]
        public localGptLibContentType Type { get; set; }

        public required string Prompt { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
