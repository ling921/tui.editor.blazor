using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ToastUI.Internals;

internal sealed class EnumValueJsonConverter<T> : JsonConverter<T> where T : Enum
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
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
