using System.Text.Json.Serialization;

namespace ToastUI;

/// <summary>
/// Represents the configuration options for the <see cref="Editor"/> component.
/// </summary>
public class EditorOptions : ViewerOptions
{
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
    /// Gets or sets the language of the editor. Default using <see cref="EditorLanguage.DefaultLanguage"/>.
    /// </summary>
    /// <remarks>
    /// When initializing the editor, it will search supported languages and fallback to <see cref="EditorLanguage.DefaultLanguage"/> if not found.
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
    /// Gets or sets the placeholder text of the editable element. Default is empty string.
    /// </summary>
    public virtual string Placeholder { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the custom WYSIWYG-markdown renderer. Default is <see langword="null"/>.
    /// </summary>
    /// <remarks>
    /// This is not implemented yet.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual object? CustomMarkdownRenderer { get; set; }

    /// <summary>
    /// Gets or sets the rules for replacing the text with widget node. Default is <see langword="null"/>.
    /// </summary>
    /// <remarks>
    /// This is not implemented yet.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual object[]? WidgetRules { get; set; }

    /// <summary>
    /// Gets or sets whether to focus the editor on creation. Default is <see langword="true"/>.
    /// </summary>
    public virtual bool Autofocus { get; set; } = true;
}
