using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json.Serialization;

namespace ToastUI;

/// <summary>
/// Represents the configuration options for the <see cref="Viewer"/> component.
/// </summary>
public class ViewerOptions
{
    /// <summary>
    /// Gets or sets the element reference that will be used to initialize the editor.
    /// </summary>
    /// <remarks>
    /// The property is set by the <see cref="Editor"/> component automatically.
    /// </remarks>
    public virtual ElementReference El { get; set; } = default!;

    /// <summary>
    /// Gets or sets the initial content for the editor.
    /// </summary>
    /// <remarks>
    /// The property is set by the <see cref="Editor"/> component using bound value.
    /// </remarks>
    public virtual string InitialValue { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the events that will be invoked from the editor.
    /// </summary>
    /// <remarks>
    /// The property is set by the <see cref="Editor"/> component automatically.
    /// </remarks>
    public virtual Dictionary<string, DotNetObjectReference<EditorInvokeHelper>> Events { get; set; } = new();

    /// <summary>
    /// Gets or sets the plugins. Default is <see langword="null"/>.
    /// </summary>
    /// <remarks>
    /// This is not implemented yet.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual object[]? Plugins { get; set; }

    /// <summary>
    /// Gets or sets whether to use the extended Autolinks specified in GFM spec. Default is <see langword="false"/>.
    /// </summary>
    public virtual bool ExtendedAutolinks { get; set; }

    /// <summary>
    /// Gets or sets the link attributes of anchor element that should be rel, target, hreflang, type. Default is <see langword="null"/>.
    /// </summary>
    /// <remarks>
    /// This is not implemented yet.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual object? LinkAttributes { get; set; }

    /// <summary>
    /// Gets or sets the custom markdown-HTML or markdown-WYSIWYG renderer. Default is <see langword="null"/>.
    /// </summary>
    /// <remarks>
    /// This is not implemented yet.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual object? CustomHTMLRenderer { get; set; }

    /// <summary>
    /// Gets or sets whether to use the specification of link reference definition. Default is <see langword="false"/>.
    /// </summary>
    public virtual bool ReferenceDefinition { get; set; }

    /// <summary>
    /// Gets or sets the custom HTML sanitizer. Default is <see langword="null"/>.
    /// </summary>
    /// <remarks>
    /// This is not implemented yet.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual object? CustomHTMLSanitizer { get; set; }

    /// <summary>
    /// Gets or sets whether to use the front matter. Default is <see langword="false"/>.
    /// </summary>
    public virtual bool FrontMatter { get; set; }

    /// <summary>
    /// Gets or sets the theme to style the editor with. Default is <see cref="EditorThemes.Light"/> which is the style of "toastui-editor.css".
    /// </summary>
    public virtual EditorThemes Theme { get; set; } = EditorThemes.Light;
}
