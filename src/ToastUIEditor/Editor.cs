using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System.Diagnostics.CodeAnalysis;

namespace ToastUI;

/// <summary>
/// The ToastUI Editor component.
/// </summary>
public partial class Editor : InputBase<string>, IAsyncDisposable
{
    /// <summary>
    /// The configuration options for the editor.
    /// </summary>
    [Parameter]
    public EditorOptions Options { get; set; } = new();

    /// <summary>
    /// The string to display when the editor is empty.
    /// </summary>
    /// <remarks>
    /// This will override the placeholder in <see cref="EditorOptions"/> if it has value.
    /// </remarks>
    [Parameter]
    public string? Placeholder { get; set; }

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
            await Create();
        }
        else
        {

        }
    }

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out string result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        result = value ?? string.Empty;
        validationErrorMessage = null;
        return true;
    }

    /// <inheritdoc/>
    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await DisposeJavaScriptObjects();

        if (Options.WidgetRules?.Length > 0)
        {
            foreach (var item in Options.WidgetRules)
            {
                item?.Dispose();
            }
        }

        (this as IDisposable).Dispose();
    }
}
