using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;

namespace ToastUI;

partial class Viewer
{
    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    private ElementReference _element;
    private IJSObjectReference? _module;
    private IJSObjectReference? _instance;

    /// <summary>
    /// Initialize the JavaScript viewer instance.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public virtual async Task Create()
    {
#if DEBUG
        _module = await JS.InvokeAsync<IJSObjectReference>("import", "./_content/ToastUIEditor/interop.js");
#else
        _module = await JS.InvokeAsync<IJSObjectReference>("import", "./_content/ToastUIEditor/interop.min.js");
#endif
        Options.Reference = DotNetObjectReference.Create(this);
        Options.Element = _element;
        Options.InitialValue = Value;

        _instance = await _module.InvokeAsync<IJSObjectReference>("ToastUI.factory", Options);
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

    [MemberNotNull(nameof(_module), nameof(_instance))]
    private void EnsureInstance()
    {
        if (_module is null || _instance is null)
        {
            throw new InvalidOperationException("The viewer has not yet been initialized.");
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
