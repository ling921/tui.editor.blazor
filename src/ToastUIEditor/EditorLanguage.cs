namespace ToastUI;

/// <summary>
/// Helper class for editor language.
/// </summary>
/// <remarks>
/// Language matching is case-insensitive, and can fallback matching ignoring region part.
/// </remarks>
public static partial class EditorLanguage
{
    /// <summary>
    /// The default language.
    /// </summary>
    public const string DEFAULT_LANGUAGE = "en-US";

    internal static string DefaultLanguage { get; private set; } = DEFAULT_LANGUAGE;
    internal static Dictionary<string, IDictionary<string, string>> Translations { get; private set; } = new(StringComparer.OrdinalIgnoreCase)
    {
        { DEFAULT_LANGUAGE, new Dictionary<string, string>() }
    };

    /// <summary>
    /// Set default language.
    /// </summary>
    /// <param name="language">The language code.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="language"/> is <see langword="null"/> or empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown if <paramref name="language"/> is not supported.</exception>
    public static void SetDefaultLanguage(string language)
    {
        ThrowHelper.ThrowIfNullOrEmpty(language);

        foreach (var key in Translations.Keys)
        {
            if (key.Equals(language, StringComparison.OrdinalIgnoreCase))
            {
                DefaultLanguage = key;
                return;
            }
        }
        throw new InvalidOperationException($"Language '{language}' is not supported. Please use 'Add(...)' method to add language.");
    }

    /// <summary>
    /// Add translation for specific language. If language already exists, it will be merged.
    /// <para>Language matching is case-insensitive, and can fallback matching ignoring region part.</para>
    /// </summary>
    /// <param name="language">The language code.</param>
    /// <param name="translation">The translation of language.</param>
    /// <param name="setAsDefault">Set this language as default language.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="language"/> is <see langword="null"/> or empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="translation"/> is <see langword="null"/>.</exception>
    public static void Add(string language, IDictionary<string, string> translation, bool setAsDefault = false)
    {
        ThrowHelper.ThrowIfNullOrEmpty(language);
        ThrowHelper.ThrowIfNull(translation);

        if (Translations.TryGetValue(language, out var originalTranslation))
        {
            foreach (var item in translation)
            {
                originalTranslation[item.Key] = item.Value;
            }
        }
        else
        {
            Translations.Add(language, translation);
        }
    }

    /// <summary>
    /// Get all supported languages.
    /// </summary>
    /// <returns>The supported languages.</returns>
    public static string[] GetSupportLanguages() => Translations.Keys.ToArray();

    /// <summary>
    /// Get language code matched with given language.
    /// </summary>
    /// <param name="language">The language code.</param>
    /// <returns>The language code matched with given language if exists, otherwise <c>en</c>.</returns>
    internal static string GetMatchedLanguage(string? language)
    {
        if (!string.IsNullOrWhiteSpace(language))
        {
            var lang = language.Trim().ToLower();
            if (Translations.ContainsKey(lang))
            {
                return lang;
            }
            if (lang.Contains('-'))
            {
                var langCode = lang.Split('-')[0];
                if (Translations.ContainsKey(langCode))
                {
                    return langCode;
                }
            }
            else
            {
                foreach (var key in Translations.Keys)
                {
                    if (key.StartsWith(lang, StringComparison.OrdinalIgnoreCase))
                    {
                        return key;
                    }
                }
            }
        }

        return DefaultLanguage;
    }
}
