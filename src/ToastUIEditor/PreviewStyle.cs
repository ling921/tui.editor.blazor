using System.Text.Json.Serialization;

namespace ToastUI;

/// <summary>
/// Represents the preview styles.
/// </summary>
[JsonConverter(typeof(EnumValueJsonConverter<PreviewStyle>))]
public enum PreviewStyle
{
    /// <summary>
    /// The tab style.
    /// </summary>
    [JsonValue("tab")]
    Tab,

    /// <summary>
    /// The vertical style.
    /// </summary>
    [JsonValue("vertical")]
    Vertical,
}
