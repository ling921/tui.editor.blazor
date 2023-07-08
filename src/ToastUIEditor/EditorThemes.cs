using System.Text.Json.Serialization;

namespace ToastUI;

/// <summary>
/// Represents the editor themes.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EditorThemes
{
    /// <summary>
    /// The default theme.
    /// </summary>
    Light,

    /// <summary>
    /// The dark theme.
    /// </summary>
    Dark,

    /// <summary>
    /// The theme is automatically selected matching the browser preference.
    /// </summary>
    Auto,
}
