using System.Text.Json.Serialization;

namespace ToastUI;

/// <summary>
/// Represents the Editor or Viewer themes.
/// </summary>
[JsonConverter(typeof(EnumValueJsonConverter<Theme>))]
public enum Theme
{
    /// <summary>
    /// The default theme.
    /// </summary>
    [JsonValue("default")]
    Light,

    /// <summary>
    /// The dark theme.
    /// </summary>
    [JsonValue("dark")]
    Dark,

    /// <summary>
    /// The theme is automatically selected matching the browser preference.
    /// </summary>
    [JsonValue("auto")]
    Auto,
}
