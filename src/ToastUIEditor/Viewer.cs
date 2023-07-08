using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;

namespace ToastUI;

/// <summary>
/// The ToastUI Viewer component.
/// </summary>
public class Viewer : ComponentBase, IAsyncDisposable
{
    [Inject] IJSRuntime JS { get; set; } = default!;
    ElementReference _element;
    IJSObjectReference? _module;
    IJSObjectReference? _instance;
    readonly List<KeyValuePair<string, DotNetObjectReference<EditorInvokeHelper>>> _eventCallbackRefs;
    string? _previousValue;

    #region Parameters

    /// <summary>
    /// Gets or sets the value of the viewer.
    /// </summary>
    [Parameter] public string Value { get; set; } = string.Empty;

    /// <summary>
    /// The configuration options for the viewer.
    /// </summary>
    [Parameter] public ViewerOptions Options { get; set; } = new();

    /// <summary>
    /// An event that is fired when the viewer is fully loaded.
    /// </summary>
    [Parameter] public EventCallback OnLoad { get; set; }

    /// <summary>
    /// An event that is fired when the viewer's content changes.
    /// </summary>
    /// <remarks>
    /// The parameter of the original event is the content type of the viewer, which is changed to the text corresponding to the content type of the viewer in this component.
    /// </remarks>
    [Parameter] public EventCallback<string> OnChange { get; set; }

    /// <summary>
    /// An event that is fired when format change by cursor position.
    /// </summary>
    /// <remarks>
    /// The parameter of the event is the content type of the viewer.
    /// </remarks>
    [Parameter] public EventCallback<string> OnCaretChange { get; set; }

    /// <summary>
    /// An event that is fired when viewer get focus.
    /// </summary>
    /// <remarks>
    /// The parameter of the event is the content type of the viewer.
    /// </remarks>
    [Parameter] public EventCallback<string> OnFocus { get; set; }

    /// <summary>
    /// An event that is fired when viewer loose focus.
    /// </summary>
    /// <remarks>
    /// The parameter of the event is the content type of the viewer.
    /// </remarks>
    [Parameter] public EventCallback<string> OnBlur { get; set; }

    /// <summary>
    /// Gets or sets whether logging is enabled. Only for debugging purposes.
    /// </summary>
    [Parameter] public bool EnableLogging { get; set; }

    /// <summary>
    /// Gets or sets a collection of additional attributes that will be applied to the created element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)] public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; } = default!;

    #endregion Parameters

    #region Constructor

    /// <summary>
    /// Initializes a new instance of <see cref="Viewer"/>.
    /// </summary>
    public Viewer()
    {
        _eventCallbackRefs = new();

        var loadRef = DotNetObjectReference.Create(new EditorInvokeHelper(async () =>
        {
            _ = Log("OnLoad is fired.");
            // JavaScript editor will fire 'OnLoad' event before '_instance' assignment. So wait for '_instance' assignment.
            var time = 3 * 1000;
            while (_instance is null && time > 0)
            {
                await Task.Delay(100);
                time -= 100;
            }
            _ = OnLoad.InvokeAsync();
        }));
        var changeRef = DotNetObjectReference.Create(new EditorInvokeHelper((editorType) =>
        {
            _ = Log("OnChange is fired.", "Editor type: ", editorType, "Value: ", Value);
            return OnChange.InvokeAsync(Value);
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

        _eventCallbackRefs.Add(new("load", loadRef));
        _eventCallbackRefs.Add(new("change", changeRef));
        _eventCallbackRefs.Add(new("caretChange", caretChangeRef));
        _eventCallbackRefs.Add(new("focus", focusRef));
        _eventCallbackRefs.Add(new("blur", blurRef));

        _ = Log("Viewer component is initialized.");
    }

    #endregion Constructor

    #region Protected Methods

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        if (_previousValue != Value)
        {
            _previousValue = Value;
            if (_instance is not null)
            {
                await SetMarkdown(Value);
            }
        }
    }

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

    /// <summary>
    /// Ensure the viewer instance is not null.
    /// </summary>
    /// <exception cref="InvalidOperationException">The viewer has not yet been initialized.</exception>
    [MemberNotNull(nameof(_module), nameof(_instance))]
    protected void EnsureInstance()
    {
        if (_module is null || _instance is null)
        {
            throw new InvalidOperationException("The viewer has not yet been initialized.");
        }
    }

    #endregion Protected Methods

    #region Public Methods

    /// <summary>
    /// Initialize the JavaScript viewer instance.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public virtual async Task Initialize()
    {
        _module = await JS.InvokeAsync<IJSObjectReference>("import", "./_content/ToastUI.Editor/interop.js");

        Options.El = _element;
        Options.Events = _eventCallbackRefs.ToDictionary(i => i.Key, i => i.Value);
        Options.InitialValue = Value ?? string.Empty;

        _instance = await _module.InvokeAsync<IJSObjectReference>("ToastUI.factory", Options);

        _ = Log("Viewer instance is created.");
    }

    /// <summary>
    /// Set markdown to viewer for preview.
    /// </summary>
    /// <param name="markdown">The markdown text.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="markdown"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">The viewer has not yet been initialized.</exception>
    public virtual ValueTask SetMarkdown(string markdown)
    {
        ThrowHelper.ThrowIfNull(markdown);

        EnsureInstance();

        return _instance!.InvokeVoidAsync("instance.setMarkdown", markdown);
    }

    #endregion Public Methods

    #region Dispose Methods

    /// <inheritdoc />
    async ValueTask IAsyncDisposable.DisposeAsync()
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

        // The editor instance has already dispose the callback reference, here is to ensure that all instances are disposed.
        foreach (var (_, item) in _eventCallbackRefs)
        {
            item.Dispose();
        }
    }

    #endregion Dispose Methods
}
