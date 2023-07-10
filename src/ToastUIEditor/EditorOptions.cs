using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json.Serialization;
using ToastUI.Extend;
using ToastUI.Internals;

namespace ToastUI;

/// <summary>
/// Represents the configuration options for the <see cref="Editor"/> component.
/// </summary>
public class EditorOptions
{
    /// <summary>
    /// Gets or sets the reference to the <see cref="Editor"/> component.
    /// </summary>
    /// <remarks>The property is set by the <see cref="Editor"/> component automatically.</remarks>
    [JsonPropertyName("ref")]
    public virtual DotNetObjectReference<Editor> Reference { get; set; } = default!;

    /// <summary>
    /// Gets or sets the element reference that will be used to initialize the editor.
    /// </summary>
    /// <remarks>The property is set by the <see cref="Editor"/> component automatically.</remarks>
    [JsonPropertyName("el")]
    public virtual ElementReference Element { get; set; } = default!;

    /// <summary>
    /// Gets or sets the height for the editor. Default is '300px'.
    /// </summary>
    /// <remarks>The height can be any css height value. example '300px', '100%', 'auto' ...</remarks>
    public virtual string Height { get; set; } = "300px";

    /// <summary>
    /// Gets or sets the minimum height for the editor. Default is '200px'.
    /// </summary>
    /// <remarks>
    /// The minimum height can be any css min-height value. example '300px', '100%', 'auto' ...
    /// </remarks>
    public virtual string MinHeight { get; set; } = "200px";

    /// <summary>
    /// Gets or sets the initial content for the editor.
    /// </summary>
    /// <remarks>The property is set by the <see cref="Editor"/> component using bound value.</remarks>
    public virtual string? InitialValue { get; set; }

    /// <summary>
    /// Gets or sets the preview style for the editor. Default is <see cref="PreviewStyle.Tab"/>.
    /// </summary>
    public virtual PreviewStyle PreviewStyle { get; set; } = PreviewStyle.Tab;

    /// <summary>
    /// Gets or sets the initial mode for the editor. Default is <see cref="EditorType.Markdown"/>.
    /// </summary>
    public virtual EditorType InitialEditType { get; set; } = EditorType.Markdown;

    /// <summary>
    /// Gets or sets the hooks for the editor.
    /// </summary>
    /// <remarks>This is not implemented yet.</remarks>
    public virtual object? Hooks { get; set; }

    /// <summary>
    /// Gets or sets the language for the editor. Default using <see cref="Editor.DefaultLanguage"/>.
    /// </summary>
    /// <remarks>
    /// When initializing the editor, it will search supported languages and fallback to <see
    /// cref="Editor.DefaultLanguage"/> if not found.
    /// </remarks>
    public virtual string? Language { get; set; }

    /// <summary>
    /// Gets or sets whether to use keyboard shortcuts to perform commands. Default is <see langword="true"/>.
    /// </summary>
    public virtual bool UseCommandShortcut { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to send hostname to google analytics. Default is <see langword="true"/>.
    /// </summary>
    public virtual bool UsageStatistics { get; set; } = true;

    /// <summary>
    /// Gets or sets the toolbar items. Default is basic toolbar items.
    /// </summary>
    public virtual string[][] ToolbarItems { get; set; } = new string[][]
        {
            new[] { "heading", "bold", "italic", "strike" },
            new[] { "hr", "quote" },
            new[] { "ul", "ol", "task", "indent", "outdent" },
            new[] { "table", "image", "link" },
            new[] { "code", "codeblock" },
            new[] { "scrollSync" }
        };

    /// <summary>
    /// Gets or sets whether to hide mode switch. Default is <see langword="false"/>.
    /// </summary>
    public virtual bool HideModeSwitch { get; set; }

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
    /// Gets or sets the placeholder text of the editable element.
    /// </summary>
    /// <remarks>If the <see cref="Editor.Placeholder"/> is set, this will be ignored.</remarks>
    public virtual string? Placeholder { get; set; }

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
    /// Gets or sets the custom WYSIWYG-markdown renderer. Default is <see langword="null"/>.
    /// </summary>
    /// <remarks>This is not implemented yet.</remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual object? CustomMarkdownRenderer { get; set; }

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
    /// Gets or sets whether to highlight the preview element corresponds to the cursor position in
    /// the markdown editor. Default is <see langword="true"/>.
    /// </summary>
    public virtual bool PreviewHighlight { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to use the front matter. Default is <see langword="false"/>.
    /// </summary>
    public virtual bool FrontMatter { get; set; }

    /// <summary>
    /// Gets or sets the rules for replacing the text with widget node. Default is <see langword="null"/>.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual WidgetRule[]? WidgetRules { get; set; }

    /// <summary>
    /// Gets or sets the theme to style the editor with. Default is <see cref="Theme.Light"/> which
    /// is the style of "toastui-editor.css".
    /// </summary>
    public virtual Theme Theme { get; set; } = Theme.Light;

    /// <summary>
    /// Gets or sets whether to focus the editor on creation. Default is <see langword="true"/>.
    /// </summary>
    public virtual bool Autofocus { get; set; } = true;
}
