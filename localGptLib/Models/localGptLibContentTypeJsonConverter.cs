using System.Text.Json;
using System.Text.Json.Serialization;
using yyLib;

namespace localGptLib
{
    public class localGptLibContentTypeJsonConverter: JsonConverter <localGptLibContentType>
    {
        public override void Write (Utf8JsonWriter writer, localGptLibContentType value, JsonSerializerOptions options) =>
            writer.WriteStringValue (yyConverter.EnumToString (value).ToLowerInvariant ());

        public override localGptLibContentType Read (ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            yyConverter.StringToEnum <localGptLibContentType> (reader.GetString ()!, ignoreCase: true);
    }
}
