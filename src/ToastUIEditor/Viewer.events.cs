using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;

namespace ToastUI;

partial class Viewer
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
    /// Do not know how to trigger this event. To watch the content changes, use <see cref="UpdatePreview"/> event.
    /// </remarks>
    [Parameter]
    public EventCallback<string> Change { get; set; }

    /// <summary>
    /// An event that is fired when markdown is updated.
    /// </summary>
    /// <remarks>
    /// The parameter of the event is the nodes' information of the viewer.
    /// </remarks>
    [Parameter]
    public EventCallback<JsonElement> UpdatePreview { get; set; }

    /// <summary>
    /// Do not call this method directly. This method is called by JavaScript.
    /// </summary>
    [JSInvokable("load")]
    public async Task InvokeLoadAsync()
    {
        // JavaScript viewer will fire 'load' event before '_instance' assignment. So wait for '_instance' assignment.
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
    public Task InvokeChangeAsync(string value)
    {
        return Change.InvokeAsync(value);
    }

    /// <summary>
    /// Do not call this method directly. This method is called by JavaScript.
    /// </summary>
    [JSInvokable("updatePreview")]
    public Task InvokeUpdatePreviewAsync(JsonElement value)
    {
        return UpdatePreview.InvokeAsync(value);
    }
}
