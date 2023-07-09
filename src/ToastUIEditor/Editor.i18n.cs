using ToastUI.Internals;

namespace ToastUI;

partial class Editor
{
    /// <summary>
    /// The default language.
    /// </summary>
    public const string DEFAULT_LANGUAGE = "en-US";

    private static string DefaultLanguage { get; set; } = DEFAULT_LANGUAGE;
    private static Dictionary<string, IDictionary<string, string>> Translations { get; set; } = new(StringComparer.OrdinalIgnoreCase)
    {
        { DEFAULT_LANGUAGE, new Dictionary<string, string>() }
    };

    /// <summary>
    /// Add or overwrite language data for the specified language code.
    /// </summary>
    /// <param name="code">Language code</param>
    /// <param name="data">Language data</param>
    /// <param name="isDefault">Whether to set the language as the default language.</param>
    public static void SetLanguage(string code, IDictionary<string, string> data, bool isDefault = false)
    {
        ThrowHelper.ThrowIfNullOrEmpty(code);
        ThrowHelper.ThrowIfNull(data);

        if (Translations.TryGetValue(code, out var translation))
        {
            foreach (var item in data)
            {
                translation[item.Key] = item.Value;
            }
        }
        else
        {
            Translations.Add(code, data);
        }

        if (isDefault)
        {
            DefaultLanguage = code;
        }
    }

    /// <summary>
    /// Set default language.
    /// </summary>
    /// <param name="code">The language code.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="code"/> is <see langword="null"/> or empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown if <paramref name="code"/> is not supported.</exception>
    public static void SetDefaultLanguage(string code)
    {
        ThrowHelper.ThrowIfNullOrEmpty(code);

        foreach (var key in Translations.Keys)
        {
            if (key.Equals(code, StringComparison.OrdinalIgnoreCase))
            {
                DefaultLanguage = key;
                return;
            }
        }
        throw new InvalidOperationException($"Language '{code}' is not supported. Please use 'Add(...)' method to add language.");
    }

    private static string GetMatchedLanguageOrDefault(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return DefaultLanguage;
        }

        string? fullMatch = null, partialMatch = null;
        foreach (var key in Translations.Keys)
        {
            if (key.Equals(code, StringComparison.OrdinalIgnoreCase))
            {
                fullMatch = key;
                break;
            }
            if (partialMatch is null && (key.StartsWith(code, StringComparison.OrdinalIgnoreCase) || code.StartsWith(key, StringComparison.OrdinalIgnoreCase)))
            {
                partialMatch = key;
            }
        }

        return fullMatch ?? partialMatch ?? DefaultLanguage;
    }
}
