using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace ToastUI;

partial class Editor
{
    /// <summary>
    /// An event that is fired when the editor is fully loaded.
    /// </summary>
    [Parameter]
    public EventCallback Load { get; set; }

    /// <summary>
    /// An event that is fired when the editor's content changes.
    /// </summary>
    /// <remarks>
    /// The parameter of the original event is the content type of the editor, which is changed to
    /// the text corresponding to the content type of the editor in this component.
    /// </remarks>
    [Parameter]
    public EventCallback<string> Change { get; set; }

    /// <summary>
    /// An event that is fired when format change by cursor position.
    /// </summary>
    /// <remarks>The parameter of the event is the content type of the editor.</remarks>
    [Parameter]
    public EventCallback<string> CaretChange { get; set; }

    /// <summary>
    /// An event that is fired when editor get focus.
    /// </summary>
    /// <remarks>The parameter of the event is the content type of the editor.</remarks>
    [Parameter]
    public EventCallback<string> Focus { get; set; }

    /// <summary>
    /// An event that is fired when editor loose focus.
    /// </summary>
    /// <remarks>The parameter of the event is the content type of the editor.</remarks>
    [Parameter]
    public EventCallback<string> Blur { get; set; }

    /// <summary>
    /// An event that is fired when the key is pressed in editor.
    /// </summary>
    /// <remarks>
    /// The original event has two parameters, the first is the content type of the editor, and the
    /// second is the keyboard event. The event only keeps the second parameter.
    /// </remarks>
    [Parameter]
    public EventCallback<KeyboardEventArgs> KeyDown { get; set; }

    /// <summary>
    /// An event that is fired when the key is released in editor.
    /// </summary>
    /// <remarks>
    /// The original event has two parameters, the first is the content type of the editor, and the
    /// second is the keyboard event. The event only keeps the second parameter.
    /// </remarks>
    [Parameter]
    public EventCallback<KeyboardEventArgs> KeyUp { get; set; }

    /// <summary>
    /// An event that is fired before rendering the markdown preview with html string.
    /// </summary>
    /// <remarks>The parameter of the event is the html string.</remarks>
    [Parameter]
    public EventCallback<string> BeforePreviewRender { get; set; }

    /// <summary>
    /// An event that is fired before converting WYSIWYG to markdown with markdown text.
    /// </summary>
    /// <remarks>The parameter of the event is the markdown text.</remarks>
    [Parameter]
    public EventCallback<string> BeforeConvertWysiwygToMarkdown { get; set; }

    /// <summary>
    /// Do not call this method directly. This method is called by JavaScript.
    /// </summary>
    [JSInvokable("load")]
    public async Task InvokeLoadAsync()
    {
        // JavaScript editor will fire 'load' event before '_instance' assignment. So wait for
        // '_instance' assignment.
        var time = 3 * 1000;
        while (_instance is null && time > 0)
        {
            await Task.Delay(100);
            time -= 100;
        }
        _ = Load.InvokeAsync();
    }

    /// <summary>
    /// Do not call this method directly. This method is called by JavaScript.
    /// </summary>
    [JSInvokable("change")]
    public Task InvokeChangeAsync(string editorType, string value)
    {
        if (value != CurrentValueAsString)
        {
            CurrentValueAsString = value;
            StateHasChanged();
            _ = ValueChanged.InvokeAsync(value);
            _ = Change.InvokeAsync(value);
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Do not call this method directly. This method is called by JavaScript.
    /// </summary>
    [JSInvokable("caretChange")]
    public Task InvokeCaretChangeAsync(string editorType)
    {
        return CaretChange.InvokeAsync(editorType);
    }

    /// <summary>
    /// Do not call this method directly. This method is called by JavaScript.
    /// </summary>
    [JSInvokable("focus")]
    public Task InvokeFocusAsync(string editorType)
    {
        return Focus.InvokeAsync(editorType);
    }

    /// <summary>
    /// Do not call this method directly. This method is called by JavaScript.
    /// </summary>
    [JSInvokable("blur")]
    public Task InvokeBlurAsync(string editorType)
    {
        return Blur.InvokeAsync(editorType);
    }

    /// <summary>
    /// Do not call this method directly. This method is called by JavaScript.
    /// </summary>
    [JSInvokable("keydown")]
    public Task InvokeKeyDownAsync(string editorType, KeyboardEventArgs e)
    {
        return KeyDown.InvokeAsync(e);
    }

    /// <summary>
    /// Do not call this method directly. This method is called by JavaScript.
    /// </summary>
    [JSInvokable("keyup")]
    public Task InvokeKeyUpAsync(string editorType, KeyboardEventArgs e)
    {
        return KeyUp.InvokeAsync(e);
    }

    /// <summary>
    /// Do not call this method directly. This method is called by JavaScript.
    /// </summary>
    [JSInvokable("beforePreviewRender")]
    public Task InvokeBeforePreviewRenderAsync(string html)
    {
        return BeforePreviewRender.InvokeAsync(html);
    }

    /// <summary>
    /// Do not call this method directly. This method is called by JavaScript.
    /// </summary>
    [JSInvokable("beforeConvertWysiwygToMarkdown")]
    public Task InvokeBeforeConvertWysiwygToMarkdownAsync(string markdown)
    {
        return BeforeConvertWysiwygToMarkdown.InvokeAsync(markdown);
    }
}
