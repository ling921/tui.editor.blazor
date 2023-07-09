using ToastUI.Internals;

namespace ToastUI.Extend;

/// <summary>
/// Represents the link attribute names.
/// </summary>
public enum LinkAttributeNames
{
    /// <summary>
    /// The rel attribute.  
    /// </summary>
    [JsonValue("rel")]
    Rel,

    /// <summary>
    /// The target attribute.
    /// </summary>
    [JsonValue("target")]
    Target,

    /// <summary>
    /// The hreflang attribute.
    /// </summary>
    [JsonValue("hreflang")]
    HrefLang,

    /// <summary>
    /// The type attribute.
    /// </summary>
    [JsonValue("type")]
    Type,
}
