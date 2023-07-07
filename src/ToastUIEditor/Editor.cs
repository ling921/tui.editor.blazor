using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;

namespace ToastUI;

/// <summary>
/// The ToastUI Editor component.
/// </summary>
public class Editor : InputBase<string>, IAsyncDisposable
{
    #region Private Properties and Fields

    [Inject] IJSRuntime JS { get; set; } = default!;
    ElementReference _element;
    IJSObjectReference? _editorModule;
    IJSObjectReference? _editorInstance;
    readonly List<KeyValuePair<string, DotNetObjectReference<EditorInvokeHelper>>> _eventCallbackRefs;

    #endregion Private Properties and Fields

    #region Parameters

    /// <summary>
    /// The configuration options for the editor.
    /// </summary>
    [Parameter] public EditorOptions Options { get; set; } = new();

    /// <summary>
    /// The string to display when the editor is empty.
    /// </summary>
    /// <remarks>
    /// This will override the placeholder in <see cref="EditorOptions"/> if it has value.
    /// </remarks>
    [Parameter] public string? Placeholder { get; set; }

    /// <summary>
    /// An event that is fired when the editor is fully loaded.
    /// </summary>
    [Parameter] public EventCallback OnLoad { get; set; }

    /// <summary>
    /// An event that is fired when the editor's content changes.
    /// </summary>
    /// <remarks>
    /// The parameter of the original event is the content type of the editor, which is changed to the text corresponding to the content type of the editor in this component.
    /// </remarks>
    [Parameter] public EventCallback<string> OnChange { get; set; }

    /// <summary>
    /// An event that is fired when format change by cursor position.
    /// </summary>
    /// <remarks>
    /// The parameter of the event is the content type of the editor.
    /// </remarks>
    [Parameter] public EventCallback<string> OnCaretChange { get; set; }

    /// <summary>
    /// An event that is fired when editor get focus.
    /// </summary>
    /// <remarks>
    /// The parameter of the event is the content type of the editor.
    /// </remarks>
    [Parameter] public EventCallback<string> OnFocus { get; set; }

    /// <summary>
    /// An event that is fired when editor loose focus.
    /// </summary>
    /// <remarks>
    /// The parameter of the event is the content type of the editor.
    /// </remarks>
    [Parameter] public EventCallback<string> OnBlur { get; set; }

    /// <summary>
    /// An event that is fired when the key is pressed in editor.
    /// </summary>
    /// <remarks>
    /// The original event has two parameters, the first is the content type of the editor, and the second is the keyboard event.
    /// The event only keeps the second parameter.
    /// </remarks>
    [Parameter] public EventCallback<KeyboardEventArgs> OnKeydown { get; set; }

    /// <summary>
    /// An event that is fired when the key is released in editor.
    /// </summary>
    /// <remarks>
    /// The original event has two parameters, the first is the content type of the editor, and the second is the keyboard event.
    /// The event only keeps the second parameter.
    /// </remarks>
    [Parameter] public EventCallback<KeyboardEventArgs> OnKeyup { get; set; }

    /// <summary>
    /// An event that is fired before rendering the markdown preview with html string.
    /// </summary>
    /// <remarks>
    /// The parameter of the event is the html string.
    /// </remarks>
    [Parameter] public EventCallback<string> BeforePreviewRender { get; set; }

    /// <summary>
    /// An event that is fired before converting wysiwyg to markdown with markdown text.
    /// </summary>
    /// <remarks>
    /// The parameter of the event is the markdown text.
    /// </remarks>
    [Parameter] public EventCallback<string> BeforeConvertWysiwygToMarkdown { get; set; }

    /// <summary>
    /// Gets or sets whether logging is enabled. Only for debugging purposes.
    /// </summary>
    [Parameter] public bool EnableLogging { get; set; }

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of <see cref="Editor"/>.
    /// </summary>
    public Editor()
    {
        _eventCallbackRefs = new();

        var loadRef = DotNetObjectReference.Create(new EditorInvokeHelper(() =>
        {
            _ = Log("OnLoad is fired.");
            return OnLoad.InvokeAsync();
        }));
        var changeRef = DotNetObjectReference.Create(new EditorInvokeHelper(async (editorType) =>
        {
            var value = await GetContent(editorType);
            _ = Log("OnChange is fired.", "Editor type: ", editorType, "Old value: ", CurrentValueAsString, "New value: ", value);
            if (value != CurrentValueAsString)
            {
                CurrentValueAsString = value;
                _ = OnChange.InvokeAsync(value);
                StateHasChanged();
            }
        }));
        var caretChangeRef = DotNetObjectReference.Create(new EditorInvokeHelper((editorType) =>
        {
            _ = Log("OnCaretChange is fired.", "Editor type: ", editorType);
            return OnCaretChange.InvokeAsync(editorType);
        }));
        var focusRef = DotNetObjectReference.Create(new EditorInvokeHelper((editorType) =>
        {
            _ = Log("OnFocus is fired.", "Editor type: ", editorType);
            return OnFocus.InvokeAsync(editorType);
        }));
        var blurRef = DotNetObjectReference.Create(new EditorInvokeHelper((editorType) =>
        {
            _ = Log("OnBlur is fired.", "Editor type: ", editorType);
            return OnBlur.InvokeAsync(editorType);
        }));
        var keyDownRef = DotNetObjectReference.Create(new EditorInvokeHelper((editorType, ev) =>
        {
            _ = Log("OnKeydown is fired.", "Editor type: ", editorType, "KeyboardEventArgs: ", ev);
            return OnKeydown.InvokeAsync(ev);
        }));
        var keyUpRef = DotNetObjectReference.Create(new EditorInvokeHelper((editorType, ev) =>
        {
            _ = Log("OnKeyup is fired.", "Editor type: ", editorType, "KeyboardEventArgs: ", ev);
            return OnKeyup.InvokeAsync(ev);
        }));
        var beforePreviewRenderRef = DotNetObjectReference.Create(new EditorInvokeHelper((html) =>
        {
            _ = Log("BeforePreviewRender is fired.", "Html: ", html);
            return BeforePreviewRender.InvokeAsync(html);
        }));
        var beforeConvertWysiwygToMarkdownRef = DotNetObjectReference.Create(new EditorInvokeHelper((markdownText) =>
        {
            _ = Log("BeforeConvertWysiwygToMarkdown is fired.", "MarkdownText: ", markdownText);
            return BeforeConvertWysiwygToMarkdown.InvokeAsync(markdownText);
        }));

        _eventCallbackRefs.Add(new("load", loadRef));
        _eventCallbackRefs.Add(new("change", changeRef));
        _eventCallbackRefs.Add(new("caretChange", caretChangeRef));
        _eventCallbackRefs.Add(new("focus", focusRef));
        _eventCallbackRefs.Add(new("blur", blurRef));
        _eventCallbackRefs.Add(new("keydown", keyDownRef));
        _eventCallbackRefs.Add(new("keyup", keyUpRef));
        _eventCallbackRefs.Add(new("beforePreviewRender", beforePreviewRenderRef));
        _eventCallbackRefs.Add(new("beforeConvertWysiwygToMarkdown", beforeConvertWysiwygToMarkdownRef));

        _ = Log("Editor component is initialized.");
    }

    #endregion Constructor

    #region Protected Methods

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var i = 0;
        builder.OpenComponent<HeadContent>(i++);
        builder.AddAttribute(i++, "ChildContent", (RenderFragment)((RenderTreeBuilder b) =>
        {
            b.AddMarkupContent(i++, "<link href='https://uicdn.toast.com/editor/latest/toastui-editor.min.css' rel='stylesheet' />");
            b.AddMarkupContent(i++, "<link href='https://uicdn.toast.com/editor/latest/theme/toastui-editor-dark.min.css' rel='stylesheet' />");
            b.AddMarkupContent(i++, "<link href='./_content/ToastUI.Editor/toastui-editor.min.css' rel='stylesheet' />");
            b.AddMarkupContent(i++, "<link href='./_content/ToastUI.Editor/theme/toastui-editor-dark.min.css' rel='stylesheet' />");
        }));
        builder.CloseComponent();

        builder.OpenElement(i++, "div");
        builder.AddMultipleAttributes(i++, AdditionalAttributes);
#if NET8_0_OR_GREATER
        if (!string.IsNullOrEmpty(NameAttributeValue))
        {
            builder.AddAttribute(i++, "name", NameAttributeValue);
        }
#endif
        if (!string.IsNullOrEmpty(CssClass))
        {
            builder.AddAttribute(i++, "class", CssClass);
        }
        builder.AddElementReferenceCapture(i++, capturedRef => _element = capturedRef);
        builder.CloseElement();
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await Initialize();
        }
    }

    /// <summary>
    /// Get content from editor by type.
    /// </summary>
    /// <param name="type">The type of content.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. The result is the content.</returns>
    protected virtual async Task<string> GetContent(string? type) => type?.ToLower() switch
    {
        "markdown" => await GetMarkdown(),
        "wysiwyg" => await GetHTML(),
        _ => string.Empty
    };

    /// <summary>
    /// Write log, for debug purpose.
    /// </summary>
    /// <param name="args">arguments</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected virtual async Task Log(params object?[] args)
    {
#if DEBUG
        if (EnableLogging)
        {
            await JS.InvokeVoidAsync("console.log", args);
        }
#endif
    }

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out string result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        result = value ?? string.Empty;
        validationErrorMessage = null;
        return true;
    }

    #endregion Protected Methods

    #region Public Methods

    /// <summary>
    /// Initialize the JavaScript editor instance.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public virtual async Task Initialize()
    {
        _editorModule = await JS.InvokeAsync<IJSObjectReference>("import", "./_content/ToastUI.Editor/interop.js");
        _ = _editorModule.InvokeVoidAsync("setLanguages", EditorLanguage.Translations);

        Options.El = _element;
        Options.Events = _eventCallbackRefs.ToDictionary(i => i.Key, i => i.Value);
        Options.InitialValue = CurrentValueAsString ?? string.Empty;
        Options.Language = EditorLanguage.GetMatchedLanguage(Options.Language);
        if (!string.IsNullOrEmpty(Placeholder))
        {
            Options.Placeholder = Placeholder;
        }

        _editorInstance = await _editorModule.InvokeAsync<IJSObjectReference>("initEditor", Options);

        _ = Log("Editor instance is created.");
    }

    /// <summary>
    /// Set language for editor.
    /// </summary>
    /// <param name="code">The code for I18N language.</param>
    /// <param name="data">The data for language.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">code is <see langword="null"/> or empty.</exception>
    /// <exception cref="ArgumentNullException">data is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask SetLanguage(string code, Dictionary<string, string> data)
    {
        ThrowHelper.ThrowIfNullOrEmpty(code);
        ThrowHelper.ThrowIfNull(data);

        EnsureEditorInstance();

        EditorLanguage.AddTranslation(code, data);
        return _editorModule.InvokeVoidAsync("setLanguage", code, data);
    }

    /// <summary>
    /// Set language for editor. Multiple languages share the same language data, like en, en-US, en-GB.
    /// </summary>
    /// <param name="codes">The code for I18N language.</param>
    /// <param name="data">The data for language.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">codes or data is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">codes is empty.</exception>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask SetLanguage(IEnumerable<string> codes, Dictionary<string, string> data)
    {
        ThrowHelper.ThrowIfNull(codes);
        ThrowHelper.ThrowIfNull(data);

        if (codes.Any())
        {
            EnsureEditorInstance();

            foreach (var code in codes)
            {
                EditorLanguage.AddTranslation(code, data);
            }
            return _editorModule.InvokeVoidAsync("setLanguage", codes, data);
        }
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Change editor's mode to given mode string.
    /// </summary>
    /// <param name="mode">The mode to change.</param>
    /// <param name="withoutFocus">If true, the editor will not be focused after mode changed.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask ChangeMode(EditorModes mode, bool withoutFocus)
    {
        EnsureEditorInstance();
        return _editorInstance.InvokeVoidAsync("changeMode", mode.ToString(), withoutFocus);
    }

    /// <summary>
    /// Determine whether the editor is markdown mode.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation. The result is <see langword="true"/> if the editor is markdown mode, otherwise <see langword="false"/>.</returns>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask<bool> IsMarkdownMode()
    {
        EnsureEditorInstance();
        return _editorInstance.InvokeAsync<bool>("isMarkdownMode");
    }

    /// <summary>
    /// Get content from editor as markdown.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation. The result is the markdown content.</returns>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask<string> GetMarkdown()
    {
        EnsureEditorInstance();
        return _editorInstance.InvokeAsync<string>("getMarkdown");
    }

    /// <summary>
    /// Set editor's content to specified markdown text.
    /// </summary>
    /// <param name="markdown">The markdown text.</param>
    /// <param name="cursorToEnd">Whether move cursor to contents end.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">markdown is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask SetMarkdown(string markdown, bool cursorToEnd)
    {
        ThrowHelper.ThrowIfNull(markdown);

        EnsureEditorInstance();
        return _editorInstance.InvokeVoidAsync("setMarkdown", markdown, cursorToEnd);
    }

    /// <summary>
    /// Determine whether the editor is WYSIWYG mode.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation. The result is <see langword="true"/> if the editor is wysiwyg mode, otherwise <see langword="false"/>.</returns>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask<bool> IsWysiwygMode()
    {
        EnsureEditorInstance();
        return _editorInstance.InvokeAsync<bool>("isWysiwygMode");
    }

    /// <summary>
    /// Get content from editor as html.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation. The result is the html content.</returns>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask<string> GetHTML()
    {
        EnsureEditorInstance();
        return _editorInstance.InvokeAsync<string>("getHTML");
    }

    /// <summary>
    /// Set editor's content to specified html text.
    /// </summary>
    /// <param name="html">The html text.</param>
    /// <param name="cursorToEnd">Whether move cursor to contents end.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">html is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask SetHTML(string html, bool cursorToEnd)
    {
        ThrowHelper.ThrowIfNull(html);

        EnsureEditorInstance();
        return _editorInstance.InvokeVoidAsync("setHTML", html, cursorToEnd);
    }

    /// <summary>
    /// Move cursor to end of editor's content.
    /// </summary>
    /// <param name="focus">Whether to focus the editor after moving cursor.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask MoveCursorToEnd(bool focus)
    {
        EnsureEditorInstance();
        return _editorInstance.InvokeVoidAsync("moveCursorToEnd", focus);
    }

    /// <summary>
    /// Move cursor to start of editor's content.
    /// </summary>
    /// <param name="focus">Whether to focus the editor after moving cursor.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask MoveCursorToStart(bool focus)
    {
        EnsureEditorInstance();
        return _editorInstance.InvokeVoidAsync("moveCursorToStart", focus);
    }

    /// <summary>
    /// Focus current editor.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask Focus()
    {
        EnsureEditorInstance();
        return _editorInstance.InvokeVoidAsync("focus");
    }

    /// <summary>
    /// Blur current editor.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask Blur()
    {
        EnsureEditorInstance();
        return _editorInstance.InvokeVoidAsync("blur");
    }

    /// <summary>
    /// Get editor's height.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation. The result is the editor's height in pixel.</returns>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask<string> GetHeight()
    {
        EnsureEditorInstance();
        return _editorInstance.InvokeAsync<string>("getHeight");
    }

    /// <summary>
    /// Set editor's height.
    /// </summary>
    /// <param name="height">The editor's height in pixel.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">height is <see langword="null"/> or empty.</exception>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask SetHeight(string height)
    {
        ThrowHelper.ThrowIfNullOrEmpty(height);

        EnsureEditorInstance();
        return _editorInstance.InvokeVoidAsync("setHeight", height);
    }

    /// <summary>
    /// Get editor's minimum height.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation. The result is the editor's min height in pixel.</returns>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask<string> GetMinHeight()
    {
        EnsureEditorInstance();
        return _editorInstance.InvokeAsync<string>("getMinHeight");
    }

    /// <summary>
    /// Set editor's minimum height.
    /// </summary>
    /// <param name="minHeight">The editor's minimum height in pixel.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">minHeight is <see langword="null"/> or empty.</exception>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask SetMinHeight(string minHeight)
    {
        ThrowHelper.ThrowIfNullOrEmpty(minHeight);

        EnsureEditorInstance();
        return _editorInstance.InvokeVoidAsync("setMinHeight", minHeight);
    }

    /// <summary>
    /// Get editor's scroll position of the editor container.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation. The result is the scrollTop value of editor container.</returns>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask<double> GetScrollTop()
    {
        EnsureEditorInstance();
        return _editorInstance.InvokeAsync<double>("getScrollTop");
    }

    /// <summary>
    /// Set editor's scroll position of the editor container.
    /// </summary>
    /// <param name="value">The scrollTop value of editor container.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">value is less than 0.</exception>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask SetScrollTop(double value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "The value must be greater than or equal to 0.");
        }

        EnsureEditorInstance();
        return _editorInstance.InvokeVoidAsync("setScrollTop", value);
    }

    /// <summary>
    /// Get selected text in editor.
    /// </summary>
    /// <param name="start">The start position.</param>
    /// <param name="end">The end position.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation. The result is the selected text.</returns>
    /// <exception cref="ArgumentOutOfRangeException">start is less than 0. -or- end is less than 0 or less than start.</exception>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask<string> GetSelectedText(int start, int end)
    {
        if (start < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(start), start, "The value must be greater than or equal to 0.");
        }
        if (end < 0 || end < start)
        {
            throw new ArgumentOutOfRangeException(nameof(end), end, "The value must be greater than or equal to 0 and greater than start.");
        }

        EnsureEditorInstance();
        return _editorInstance.InvokeAsync<string>("getSelectedText", start, end);
    }

    /// <summary>
    /// Get current selection range in editor.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation. The result is the selection range.</returns>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized, or the editor is neither in markdown mode nor in wysiwyg mode.</exception>
    public virtual async ValueTask<(int CursorStart, int CursorEnd)> GetSelection()
    {
        EnsureEditorInstance();

        if (await IsMarkdownMode())
        {
            var range = await _editorInstance.InvokeAsync<int[][]>("getSelection");
            return (range[0][1], range[1][1]);
        }
        else if (await IsWysiwygMode())
        {
            var range = await _editorInstance.InvokeAsync<int[]>("getSelection");
            return (range[0], range[1]);
        }
        else
        {
            throw new InvalidOperationException("The editor is neither in markdown mode nor in wysiwyg mode.");
        }
    }

    /// <summary>
    /// Set selection range in editor.
    /// </summary>
    /// <param name="start">The start position.</param>
    /// <param name="end">The end position.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">start is less than 0. -or- end is less than 0 or less than start.</exception>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask SetSelection(int start, int end)
    {
        if (start < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(start), start, "The value must be greater than or equal to 0.");
        }
        if (end < 0 || end < start)
        {
            throw new ArgumentOutOfRangeException(nameof(end), end, "The value must be greater than or equal to 0 and greater than start.");
        }

        EnsureEditorInstance();
        return _editorInstance.InvokeVoidAsync("setSelection", start, end);
    }

    /// <summary>
    /// Insert text content to current cursor position.
    /// </summary>
    /// <param name="text">The text content.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">text is <see langword="null"/> or empty.</exception>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask InsertText(string text)
    {
        ThrowHelper.ThrowIfNullOrEmpty(text);

        EnsureEditorInstance();
        return _editorInstance.InvokeVoidAsync("insertText", text);
    }

    /// <summary>
    /// Replace current selection with text content.
    /// </summary>
    /// <param name="text">The text content.</param>
    /// <param name="start">The start position.</param>
    /// <param name="end">The end position.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">text is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">start is less than 0. -or- end is less than 0 or less than start.</exception>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask ReplaceSelection(string text, int start, int end)
    {
        ThrowHelper.ThrowIfNull(text);
        if (start < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(start), start, "The value must be greater than or equal to 0.");
        }
        if (end < 0 || end < start)
        {
            throw new ArgumentOutOfRangeException(nameof(end), end, "The value must be greater than or equal to 0 and greater than start.");
        }

        EnsureEditorInstance();
        return _editorInstance.InvokeVoidAsync("replaceSelection", text, start, end);
    }

    /// <summary>
    /// Hide editor.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask Hide()
    {
        EnsureEditorInstance();
        return _editorInstance.InvokeVoidAsync("hide");
    }

    /// <summary>
    /// Show editor.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask Show()
    {
        EnsureEditorInstance();
        return _editorInstance.InvokeVoidAsync("show");
    }

    /// <summary>
    /// Reset editor.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask Reset()
    {
        EnsureEditorInstance();
        return _editorInstance.InvokeVoidAsync("reset");
    }

    #endregion Public Methods

    #region Private Methods

    /// <summary>
    /// Ensure the editor instance is not null.
    /// </summary>
    /// <exception cref="InvalidOperationException">The editor has not yet been initialized.</exception>
    [MemberNotNull(nameof(_editorModule), nameof(_editorInstance))]
    private void EnsureEditorInstance()
    {
        if (_editorModule is null || _editorInstance is null)
        {
            throw new InvalidOperationException("The editor has not yet been initialized.");
        }
    }

    #endregion Private Methods

    #region Dispose Methods

    /// <inheritdoc/>
    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        _ = Log("DisposeAsync");

        if (_editorInstance is not null)
        {
            await _editorInstance.InvokeVoidAsync("destroy");
            await _editorInstance.DisposeAsync();
        }
        if (_editorModule is not null)
        {
            await _editorModule.DisposeAsync();
        }

        // The editor instance has already dispose the callback reference, here is to ensure that all instances are disposed.
        foreach (var (_, item) in _eventCallbackRefs)
        {
            item.Dispose();
        }

        base.Dispose(true);
    }

    #endregion Dispose Methods
}