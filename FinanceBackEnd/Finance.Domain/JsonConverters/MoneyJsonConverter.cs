using System.Text.Json;
using System.Text.Json.Serialization;
using Finance.Domain.SpecialTypes;

namespace Finance.Domain.JsonConverters;

public class MoneyJsonConverter : JsonConverter<Money>
{
    public override Money Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return new Money(reader.GetDecimal());
        }

        throw new JsonException($"Unable to convert \"{reader.GetString()}\" to Money.");
    }

    public override void Write(Utf8JsonWriter writer, Money value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }
}
