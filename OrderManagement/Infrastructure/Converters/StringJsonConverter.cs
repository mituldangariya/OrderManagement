using System.Text.Json;
using System.Text.Json.Serialization;

namespace OrderManagement.Infrastructure.Converters
{
    public class StringJsonConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.String => reader.GetString()!,
                JsonTokenType.Number => reader.GetDouble().ToString(), // or GetInt64()
                _ => throw new JsonException($"Unexpected token {reader.TokenType} for string property")
            };
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}
