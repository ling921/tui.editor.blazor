using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ToastUI;

internal sealed class EnumValueJsonConverter<T> : JsonConverter<T> where T : Enum
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected a string value for {typeof(T).Name}.");
        }

        var value = reader.GetString();
        foreach (var member in typeof(T).GetMembers(BindingFlags.Public | BindingFlags.Static))
        {
            var attribute = member.GetCustomAttribute<JsonValueAttribute>();
            if (attribute?.Value.Equals(value, StringComparison.OrdinalIgnoreCase) == true ||
                member.Name.Equals(value, StringComparison.OrdinalIgnoreCase))
            {
                return (T)Enum.Parse(typeof(T), member.Name);
            }
        }

        throw new JsonException($"Unknown {typeof(T).Name} value '{value}'.");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var attribute = value.GetType()
            .GetMember(value.ToString())[0]
            .GetCustomAttribute<JsonValueAttribute>();

        writer.WriteStringValue(attribute?.Value ?? value.ToString());
    }
}

internal sealed class EnumDictionaryKeyJsonConverter<TKey, TValue> : JsonConverter<Dictionary<TKey, TValue>> where TKey : Enum
{
    public override Dictionary<TKey, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        if (document.RootElement.ValueKind != JsonValueKind.Object)
        {
            throw new JsonException($"Expected an object for {typeof(TKey).Name} dictionary.");
        }

        var result = new Dictionary<TKey, TValue>();
        foreach (var property in document.RootElement.EnumerateObject())
        {
            TKey key = default!;
            var found = false;
            foreach (TKey item in Enum.GetValues(typeof(TKey)))
            {
                var member = typeof(TKey).GetMember(item.ToString())[0];
                var attribute = member.GetCustomAttribute<JsonValueAttribute>();
                if (attribute?.Value.Equals(property.Name, StringComparison.OrdinalIgnoreCase) == true ||
                    item.ToString().Equals(property.Name, StringComparison.OrdinalIgnoreCase))
                {
                    key = item;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                throw new JsonException($"Unknown {typeof(TKey).Name} value '{property.Name}'.");
            }

            result[key] = property.Value.Deserialize<TValue>(options)!;
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<TKey, TValue> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var item in value)
        {
            var attribute = item.Key.GetType()
                .GetMember(item.Key.ToString())[0]
                .GetCustomAttribute<JsonValueAttribute>();

            writer.WritePropertyName(attribute?.Value ?? item.Key.ToString());
            JsonSerializer.Serialize(writer, item.Value, options);
        }

        writer.WriteEndObject();
    }
}

[AttributeUsage(AttributeTargets.Field)]
internal sealed class JsonValueAttribute : Attribute
{
    public string Value { get; }

    public JsonValueAttribute(string value)
    {
        Value = value;
    }
}
