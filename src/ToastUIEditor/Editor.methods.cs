using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;
using ToastUI.Internals;

namespace ToastUI;

partial class Editor
{
    [Inject]
    private IJSRuntime JS { get; set; } = default!;
    private ElementReference _element;
    private IJSObjectReference? _module;
    private IJSObjectReference? _instance;

    /// <summary>
    /// Initialize the JavaScript editor instance.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public virtual async Task Create()
    {
#if DEBUG
        _module = await JS.InvokeAsync<IJSObjectReference>("import", "./_content/ToastUIEditor/interop.js");
#else
        _module = await JS.InvokeAsync<IJSObjectReference>("import", "./_content/ToastUIEditor/interop.min.js");
#endif
        _ = _module.InvokeVoidAsync("ToastUI.setLanguages", Translations).AsTask();

        Options.Reference = DotNetObjectReference.Create(this);
        Options.Element = _element;
        Options.InitialValue = Value;
        Options.Language = GetMatchedLanguageOrDefault(Options.Language);
        if (!string.IsNullOrEmpty(Placeholder))
        {
            Options.Placeholder = Placeholder;
        }

        _instance = await _module.InvokeAsync<IJSObjectReference>("ToastUI.factory", Options);
    }

    /// <summary>
    /// Change editor's mode to given mode string.
    /// </summary>
    /// <param name="mode">The mode to change.</param>
    /// <param name="withoutFocus">If true, the editor will not be focused after mode changed.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask ChangeMode(EditorType mode, bool withoutFocus)
    {
        EnsureInstance();
        return _instance.InvokeVoidAsync("instance.changeMode", mode.ToString(), withoutFocus);
    }

    /// <summary>
    /// Determine whether the editor is markdown mode.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation. The result is <see langword="true"/> if the editor is markdown mode, otherwise <see langword="false"/>.</returns>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask<bool> IsMarkdownMode()
    {
        EnsureInstance();
        return _instance.InvokeAsync<bool>("instance.isMarkdownMode");
    }

    /// <summary>
    /// Get content from editor as markdown.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation. The result is the markdown content.</returns>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask<string> GetMarkdown()
    {
        EnsureInstance();
        return _instance.InvokeAsync<string>("instance.getMarkdown");
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

        EnsureInstance();
        return _instance.InvokeVoidAsync("instance.setMarkdown", markdown, cursorToEnd);
    }

    /// <summary>
    /// Determine whether the editor is WYSIWYG mode.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation. The result is <see langword="true"/> if the editor is wysiwyg mode, otherwise <see langword="false"/>.</returns>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask<bool> IsWysiwygMode()
    {
        EnsureInstance();
        return _instance.InvokeAsync<bool>("instance.isWysiwygMode");
    }

    /// <summary>
    /// Get content from editor as html.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation. The result is the html content.</returns>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask<string> GetHTML()
    {
        EnsureInstance();
        return _instance.InvokeAsync<string>("instance.getHTML");
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

        EnsureInstance();
        return _instance.InvokeVoidAsync("instance.setHTML", html, cursorToEnd);
    }

    /// <summary>
    /// Move cursor to end of editor's content.
    /// </summary>
    /// <param name="focus">Whether to focus the editor after moving cursor.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask MoveCursorToEnd(bool focus)
    {
        EnsureInstance();
        return _instance.InvokeVoidAsync("instance.moveCursorToEnd", focus);
    }

    /// <summary>
    /// Move cursor to start of editor's content.
    /// </summary>
    /// <param name="focus">Whether to focus the editor after moving cursor.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask MoveCursorToStart(bool focus)
    {
        EnsureInstance();
        return _instance.InvokeVoidAsync("instance.moveCursorToStart", focus);
    }

    /// <summary>
    /// Focus current editor.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask SetFocus()
    {
        EnsureInstance();
        return _instance.InvokeVoidAsync("instance.focus");
    }

    /// <summary>
    /// Blur current editor.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask SetBlur()
    {
        EnsureInstance();
        return _instance.InvokeVoidAsync("instance.blur");
    }

    /// <summary>
    /// Get editor's height.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation. The result is the editor's height in pixel.</returns>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask<string> GetHeight()
    {
        EnsureInstance();
        return _instance.InvokeAsync<string>("instance.getHeight");
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

        EnsureInstance();
        return _instance.InvokeVoidAsync("instance.setHeight", height);
    }

    /// <summary>
    /// Get editor's minimum height.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation. The result is the editor's min height in pixel.</returns>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask<string> GetMinHeight()
    {
        EnsureInstance();
        return _instance.InvokeAsync<string>("instance.getMinHeight");
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

        EnsureInstance();
        return _instance.InvokeVoidAsync("instance.setMinHeight", minHeight);
    }

    /// <summary>
    /// Get editor's scroll position of the editor container.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation. The result is the scrollTop value of editor container.</returns>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask<double> GetScrollTop()
    {
        EnsureInstance();
        return _instance.InvokeAsync<double>("instance.getScrollTop");
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

        EnsureInstance();
        return _instance.InvokeVoidAsync("instance.setScrollTop", value);
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

        EnsureInstance();
        return _instance.InvokeAsync<string>("instance.getSelectedText", start, end);
    }

    /// <summary>
    /// Get current selection range in editor.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation. The result is the selection range.</returns>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized, or the editor is neither in markdown mode nor in wysiwyg mode.</exception>
    public virtual async ValueTask<(int CursorStart, int CursorEnd)> GetSelection()
    {
        EnsureInstance();

        if (await IsMarkdownMode())
        {
            var range = await _instance.InvokeAsync<int[][]>("instance.getSelection");
            return (range[0][1], range[1][1]);
        }
        else if (await IsWysiwygMode())
        {
            var range = await _instance.InvokeAsync<int[]>("instance.getSelection");
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

        EnsureInstance();
        return _instance.InvokeVoidAsync("instance.setSelection", start, end);
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

        EnsureInstance();
        return _instance.InvokeVoidAsync("instance.insertText", text);
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

        EnsureInstance();
        return _instance.InvokeVoidAsync("instance.replaceSelection", text, start, end);
    }

    /// <summary>
    /// Hide editor.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask Hide()
    {
        EnsureInstance();
        return _instance.InvokeVoidAsync("instance.hide");
    }

    /// <summary>
    /// Show editor.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask Show()
    {
        EnsureInstance();
        return _instance.InvokeVoidAsync("instance.show");
    }

    /// <summary>
    /// Reset editor.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">The editor instance is not initialized.</exception>
    public virtual ValueTask Reset()
    {
        EnsureInstance();
        return _instance.InvokeVoidAsync("instance.reset");
    }

    [MemberNotNull(nameof(_module), nameof(_instance))]
    private void EnsureInstance()
    {
        if (_module is null || _instance is null)
        {
            throw new InvalidOperationException("The editor has not yet been initialized.");
        }
    }

    private async Task DisposeJavaScriptObjects()
    {
        if (_instance is not null)
        {
            await _instance.InvokeVoidAsync("destroy");
            await _instance.DisposeAsync();
        }
        if (_module is not null)
        {
            await _module.DisposeAsync();
        }
    }
}
