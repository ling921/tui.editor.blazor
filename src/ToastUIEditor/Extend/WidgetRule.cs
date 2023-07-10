using Microsoft.JSInterop;
using System.Text.Json.Serialization;

namespace ToastUI.Extend;

/// <summary>
/// Represents a rule for a widget.
/// </summary>
public class WidgetRule : IDisposable
{
    private bool _disposed;

    /// <summary>
    /// Gets the regular expression pattern for the rule.
    /// </summary>
    public string Rule { get; }

    /// <summary>
    /// Gets the delegate that converts the matched text to HTML.
    /// </summary>
    [JsonIgnore]
    public Func<string, string> Delegate { get; }

    /// <summary>
    /// Gets the reference to the <see cref="WidgetRule"/> object.
    /// </summary>
    [JsonPropertyName("ref")]
    public DotNetObjectReference<WidgetRule> Reference { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WidgetRule"/> class.
    /// </summary>
    /// <param name="rule">The regular expression pattern for the rule.</param>
    /// <param name="toDOM">The delegate that converts the matched text to HTML.</param>
    public WidgetRule(string rule, Func<string, string> toDOM)
    {
        Rule = rule;
        Delegate = toDOM;
        Reference = DotNetObjectReference.Create(this);
    }

    /// <summary>
    /// Converts the matched text to HTML.
    /// </summary>
    [JSInvokable("toDOM")]
    public string ToDOM(string text) => Delegate(text);

    /// <summary>
    /// Releases all resources used by the current instance of the <see cref="WidgetRule"/> class.
    /// </summary>
    /// <param name="disposing">
    /// <see langword="true"/> to release both managed and unmanaged resources; <see
    /// langword="false"/> to release only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                Reference.Dispose();
            }
            _disposed = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
