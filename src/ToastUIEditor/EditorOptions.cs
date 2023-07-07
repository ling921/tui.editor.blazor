using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json.Serialization;
using ToastUI;

namespace ToastUI;

/// <summary>
/// Represents the configuration options for the <see cref="Editor"/> component.
/// </summary>
public class EditorOptions
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
    /// Gets or sets the height of the editor. Default is '300px'.
    /// </summary>
    /// <remarks>
    /// The height can be any css height value. example '300px', '100%', 'auto' ...
    /// </remarks>
    public virtual string Height { get; set; } = "300px";

    /// <summary>
    /// Gets or sets the minimum height of the editor. Default is '200px'.
    /// </summary>
    /// <remarks>
    /// The minimum height can be any css min-height value. example '300px', '100%', 'auto' ...
    /// </remarks>
    public virtual string MinHeight { get; set; } = "200px";

    /// <summary>
    /// Gets or sets the preview style of the editor. Default is <see cref="PreviewStyles.Tab"/>.
    /// </summary>
    public virtual PreviewStyles PreviewStyle { get; set; } = PreviewStyles.Tab;

    /// <summary>
    /// Gets or sets whether to highlight the preview element corresponds to the cursor position in the markdown editor. Default is <see langword="true"/>.
    /// </summary>
    public virtual bool PreviewHighlight { get; set; } = true;

    /// <summary>
    /// Gets or sets the initial mode of the editor. Default is <see cref="EditorModes.Markdown"/>.
    /// </summary>
    public virtual EditorModes InitialEditType { get; set; } = EditorModes.Markdown;

    /// <summary>
    /// Gets or sets whether to set the editor as viewer. Default is <see langword="false"/>.
    /// </summary>
    /// <remarks>
    /// The viewer cannot edit the content.
    /// </remarks>
    public virtual bool Viewer { get; set; } = true;

    /// <summary>
    /// Gets or sets the events that will be invoked from the editor.
    /// </summary>
    /// <remarks>
    /// The property is set by the <see cref="Editor"/> component automatically.
    /// </remarks>
    public virtual Dictionary<string, DotNetObjectReference<EditorInvokeHelper>> Events { get; set; } = new();

    /// <summary>
    /// Gets or sets the language of the editor. Default is 'en-US'.
    /// </summary>
    /// <remarks>
    /// The code for I18N language.
    /// </remarks>
    public virtual string Language { get; set; } = "en-US";

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
    /// Gets or sets the placeholder text of the editable element. Default is empty string.
    /// </summary>
    public virtual string Placeholder { get; set; } = string.Empty;

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
    /// Gets or sets the custom WYSIWYG-markdown renderer. Default is <see langword="null"/>.
    /// </summary>
    /// <remarks>
    /// This is not implemented yet.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual object? CustomMarkdownRenderer { get; set; }

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
    /// Gets or sets the rules for replacing the text with widget node. Default is <see langword="null"/>.
    /// </summary>
    /// <remarks>
    /// This is not implemented yet.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual object[]? WidgetRules { get; set; }

    /// <summary>
    /// Gets or sets the theme to style the editor with. Default is 'light' aka the 'toastui-editor.css'.
    /// </summary>
    /// <remarks>
    /// The theme can be 'light' or 'dark'.
    /// </remarks>
    public virtual string Theme { get; set; } = "light";

    /// <summary>
    /// Gets or sets whether to focus the editor on creation. Default is <see langword="true"/>.
    /// </summary>
    public virtual bool Autofocus { get; set; } = true;
}
