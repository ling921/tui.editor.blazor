using System.Text.Json.Serialization;

namespace ToastUI;

/// <summary>
/// Represents the preview styles.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PreviewStyles
{
    /// <summary>
    /// The tab style.
    /// </summary>
    Tab,

    /// <summary>
    /// The vertical style.
    /// </summary>
    Vertical,
}
