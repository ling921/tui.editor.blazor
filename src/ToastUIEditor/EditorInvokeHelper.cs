using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace ToastUI;

/// <summary>
/// A helper class for invoking events' callbacks from JavaScript.
/// </summary>
public class EditorInvokeHelper
{
    /// <summary>
    /// The callback function.
    /// </summary>
    protected readonly Func<string?, KeyboardEventArgs?, Task> Func;

    /// <summary>
    /// Initializes a new instance of the <see cref="EditorInvokeHelper"/> class.
    /// </summary>
    /// <param name="func">The callback function.</param>
    public EditorInvokeHelper(Func<Task> func)
    {
        ArgumentNullException.ThrowIfNull(func);
        Func = (_, _) => func();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditorInvokeHelper"/> class.
    /// </summary>
    /// <param name="func">The callback function.</param>
    public EditorInvokeHelper(Func<string?, Task> func)
    {
        ArgumentNullException.ThrowIfNull(func);
        Func = (p1, _) => func(p1);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditorInvokeHelper" /> class.
    /// </summary>
    /// <param name="func">The callback function.</param>
    public EditorInvokeHelper(Func<string?, KeyboardEventArgs?, Task> func)
    {
        ArgumentNullException.ThrowIfNull(func);
        Func = func;
    }

    /// <summary>
    /// Invokes the callback function.
    /// </summary>
    /// <param name="p1">The first parameter.</param>
    /// <param name="p2">The second parameter.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [JSInvokable]
    public Task InvokeAsync(string? p1 = default, KeyboardEventArgs? p2 = default)
    {
        return Func(p1, p2);
    }
}
