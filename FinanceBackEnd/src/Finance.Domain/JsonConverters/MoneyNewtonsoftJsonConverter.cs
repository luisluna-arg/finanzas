using Finance.Domain.SpecialTypes;
using Newtonsoft.Json;

namespace Finance.Domain.JsonConverters;

public class MoneyNewtonsoftJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Money);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Integer || reader.TokenType == JsonToken.Float)
        {
            var value = Convert.ToDecimal(reader.Value);
            return new Money(value);
        }

        throw new JsonSerializationException($"Unable to convert \"{reader.Value}\" to Money.");
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is Money money)
        {
            writer.WriteValue(money.Value);
        }
    }
}
