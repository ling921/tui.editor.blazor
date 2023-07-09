using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;
using ToastUI.Internals;

namespace ToastUI;

/// <summary>
/// The ToastUI Viewer component.
/// </summary>
public partial class Viewer : ComponentBase, IAsyncDisposable
{
    string? _previousValue;

    /// <summary>
    /// Gets or sets the value of the viewer.
    /// </summary>
    [Parameter] public string Value { get; set; } = string.Empty;

    /// <summary>
    /// The configuration options for the viewer.
    /// </summary>
    [Parameter] public ViewerOptions Options { get; set; } = new();

    /// <summary>
    /// Gets or sets a collection of additional attributes that will be applied to the created element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)] public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; } = default!;

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
            b.AddMarkupContent(i++, "<link href='./_content/ToastUIEditor/toastui-editor.min.css' rel='stylesheet' />");
            b.AddMarkupContent(i++, "<link href='./_content/ToastUIEditor/theme/toastui-editor-dark.min.css' rel='stylesheet' />");
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
            await Create();
        }
    }

    /// <inheritdoc />
    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await DisposeJavaScriptObjects();
    }
}
