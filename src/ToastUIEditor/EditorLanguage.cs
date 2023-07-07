namespace ToastUI;

/// <summary>
/// Helper class for editor language.
/// </summary>
internal static partial class EditorLanguage
{
    internal static Dictionary<string, IDictionary<string, string>> Translations { get; private set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Add translation for specific language. If language already exists, it will be merged.
    /// </summary>
    /// <param name="language">The language code.</param>
    /// <param name="translation">The translation of language.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="language"/> is <see langword="null"/> or empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="translation"/> is <see langword="null"/>.</exception>
    public static void AddTranslation(string language, IDictionary<string, string> translation)
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
            Translations.Add(language.ToLower(), translation);
        }
    }

    /// <summary>
    /// Get translation of language.
    /// </summary>
    /// <param name="language">The language code.</param>
    /// <returns>The translation of language if exists, otherwise <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="language"/> is <see langword="null"/> or empty.</exception>
    public static IDictionary<string, string>? GetTranslation(string language)
    {
        ThrowHelper.ThrowIfNullOrEmpty(language);

        return Translations.TryGetValue(language, out var translation) ? translation : null;
    }

    /// <summary>
    /// Get language code matched with given language.
    /// </summary>
    /// <param name="language">The language code.</param>
    /// <returns>The language code matched with given language if exists, otherwise <c>en</c>.</returns>
    internal static string GetMatchedLanguage(string language)
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
                    if (key.StartsWith(lang))
                    {
                        return key;
                    }
                }
            }
        }

        return "en";
    }
}
