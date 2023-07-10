using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json.Serialization;
using ToastUI.Extend;
using ToastUI.Internals;

namespace ToastUI;

/// <summary>
/// Represents the configuration options for the <see cref="Viewer"/> component.
/// </summary>
public class ViewerOptions
{
    /// <summary>
    /// Gets or sets the reference for the <see cref="Viewer"/> component.
    /// </summary>
    /// <remarks>The property is set by the <see cref="Viewer"/> component automatically.</remarks>
    [JsonPropertyName("ref")]
    public virtual DotNetObjectReference<Viewer> Reference { get; set; } = default!;

    /// <summary>
    /// Gets or sets the element reference that will be used to initialize the viewer.
    /// </summary>
    /// <remarks>The property is set by the <see cref="Viewer"/> component automatically.</remarks>
    [JsonPropertyName("el")]
    public virtual ElementReference Element { get; set; } = default!;

    /// <summary>
    /// Gets or sets the initial content for the viewer.
    /// </summary>
    /// <remarks>The property is set by the <see cref="Viewer"/> component using bound value.</remarks>
    public virtual string? InitialValue { get; set; }

    /// <summary>
    /// Gets whether to initialize the viewer.
    /// </summary>
    public virtual bool Viewer => true;

    /// <summary>
    /// Gets or sets the plugins. Default is <see langword="null"/>.
    /// </summary>
    /// <remarks>This is not implemented yet.</remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual object[]? Plugins { get; set; }

    /// <summary>
    /// Gets or sets whether to use the extended Autolinks specified in GFM spec. Default is <see langword="false"/>.
    /// </summary>
    public virtual bool ExtendedAutolinks { get; set; }

    /// <summary>
    /// Gets or sets the link attributes of anchor element that should be rel, target, hreflang,
    /// type. Default is <see langword="null"/>.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(EnumDictionaryKeyJsonConverter<LinkAttributeNames, string>))]
    public virtual Dictionary<LinkAttributeNames, string>? LinkAttributes { get; set; }

    /// <summary>
    /// Gets or sets the custom markdown-HTML or markdown-WYSIWYG renderer. Default is <see langword="null"/>.
    /// </summary>
    /// <remarks>This is not implemented yet.</remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual object? CustomHTMLRenderer { get; set; }

    /// <summary>
    /// Gets or sets whether to use the specification of link reference definition. Default is <see langword="false"/>.
    /// </summary>
    public virtual bool ReferenceDefinition { get; set; }

    /// <summary>
    /// Gets or sets the custom HTML sanitizer. Default is <see langword="null"/>.
    /// </summary>
    /// <remarks>This is not implemented yet.</remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual object? CustomHTMLSanitizer { get; set; }

    /// <summary>
    /// Gets or sets whether to use the front matter. Default is <see langword="false"/>.
    /// </summary>
    public virtual bool FrontMatter { get; set; }

    /// <summary>
    /// Gets or sets whether to send hostname to google analytics. Default is <see langword="true"/>.
    /// </summary>
    public virtual bool UsageStatistics { get; set; } = true;

    /// <summary>
    /// Gets or sets the theme to style the viewer with. Default is <see cref="Theme.Light"/> which
    /// is the style of "toastui-editor.css".
    /// </summary>
    public virtual Theme Theme { get; set; } = Theme.Light;
}
