using System.Runtime.CompilerServices;

namespace ToastUI.Internals;

/// <summary>
/// Helper class for exception throwing.
/// </summary>
internal static class ThrowHelper
{
    /// <summary>
    /// Throws <see cref="ArgumentException"/> if <paramref name="value"/> is <see langword="null"/>
    /// or empty.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="paramName">The parameter name.</param>
    public static void ThrowIfNullOrEmpty(string? value, [CallerArgumentExpression(nameof(value))] string paramName = "")
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("Value cannot be null or empty.", paramName);
        }
    }

    /// <summary>
    /// Throws <see cref="ArgumentNullException"/> if <paramref name="value"/> is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="paramName">The parameter name.</param>
    public static void ThrowIfNull<T>(T? value, [CallerArgumentExpression(nameof(value))] string paramName = "")
    {
        if (value is null)
        {
            throw new ArgumentNullException(paramName);
        }
    }
}
