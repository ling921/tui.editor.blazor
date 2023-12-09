using System.Text.Json.Serialization;

namespace ToastUI;

/// <summary>
/// Represents the editor modes.
/// </summary>
[JsonConverter(typeof(EnumValueJsonConverter<EditorType>))]
public enum EditorType
{
    /// <summary>
    /// The markdown mode.
    /// </summary>
    [JsonValue("markdown")]
    Markdown,

    /// <summary>
    /// The what-you-see-is-what-you-get mode.
    /// </summary>
    [JsonValue("wysiwyg")]
    WYSIWYG,
}
