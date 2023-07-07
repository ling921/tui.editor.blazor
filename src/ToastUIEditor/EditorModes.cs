using System.Text.Json.Serialization;

namespace ToastUI;

/// <summary>
/// Represents the editor modes.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EditorModes
{
    /// <summary>
    /// The markdown mode.
    /// </summary>
    Markdown,

    /// <summary>
    /// The what-you-see-is-what-you-get mode.
    /// </summary>
    WYSIWYG,
}
