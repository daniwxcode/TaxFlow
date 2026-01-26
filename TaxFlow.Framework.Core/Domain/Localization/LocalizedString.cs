namespace Core.Domain.Localization;

/// <summary>
/// Represents a string that can have translations in multiple languages.
/// Default language is French (fr-FR).
/// </summary>
public sealed class LocalizedString
{
    private readonly Dictionary<string, string> _translations = new(StringComparer.OrdinalIgnoreCase);
    private readonly string _defaultValue;

    /// <summary>
    /// Creates a new localized string with a default (French) value.
    /// </summary>
    private LocalizedString(string defaultValue)
    {
        _defaultValue = defaultValue ?? string.Empty;
        _translations["fr-FR"] = _defaultValue;
    }

    /// <summary>
    /// Creates a new localized string with a default value.
    /// </summary>
    public static LocalizedString Create(string defaultValue) => new(defaultValue);

    /// <summary>
    /// Creates a localized string with translations.
    /// </summary>
    public static LocalizedString Create(string defaultValue, params (string culture, string value)[] translations)
    {
        var ls = new LocalizedString(defaultValue);
        foreach (var (culture, value) in translations)
        {
            ls._translations[culture] = value;
        }
        return ls;
    }

    /// <summary>
    /// Adds an English translation.
    /// </summary>
    public LocalizedString En(string value)
    {
        _translations["en-US"] = value;
        return this;
    }

    /// <summary>
    /// Adds an Arabic translation.
    /// </summary>
    public LocalizedString Ar(string value)
    {
        _translations["ar-SA"] = value;
        return this;
    }

    /// <summary>
    /// Adds a Portuguese translation.
    /// </summary>
    public LocalizedString Pt(string value)
    {
        _translations["pt-PT"] = value;
        return this;
    }

    /// <summary>
    /// Adds a Spanish translation.
    /// </summary>
    public LocalizedString Es(string value)
    {
        _translations["es-ES"] = value;
        return this;
    }

    /// <summary>
    /// Adds a translation for a specific culture.
    /// </summary>
    public LocalizedString With(string culture, string value)
    {
        _translations[culture] = value;
        return this;
    }

    /// <summary>
    /// Gets the value for the specified culture, or the default value if not found.
    /// </summary>
    public string GetValue(string? culture = null)
    {
        culture ??= LocalizationContext.CurrentCulture;

        // Try exact match
        if (_translations.TryGetValue(culture, out var exact))
        {
            return exact;
        }

        // Try language only (e.g., "fr" from "fr-FR")
        var lang = culture.Split('-')[0];
        var match = _translations.Keys.FirstOrDefault(k => k.StartsWith(lang, StringComparison.OrdinalIgnoreCase));
        return match is not null ? _translations[match] : _defaultValue;
    }

    /// <summary>
    /// Gets all available translations.
    /// </summary>
    public IReadOnlyDictionary<string, string> Translations => _translations.AsReadOnly();

    /// <summary>
    /// Gets the default (French) value.
    /// </summary>
    public string Default => _defaultValue;

    /// <summary>
    /// Implicit conversion to string using current culture.
    /// </summary>
    public static implicit operator string(LocalizedString ls) => ls.GetValue();

    /// <summary>
    /// Implicit conversion from string to create a simple localized string.
    /// </summary>
    public static implicit operator LocalizedString(string value) => Create(value);

    /// <summary>
    /// Returns the localized string value for the current language context.
    /// </summary>
    /// <returns>The localized string representation.</returns>
    /// <remarks>
    /// This method calls GetValue() to retrieve the appropriate localized text based on the current language context.
    /// </remarks>
    public override string ToString() => GetValue();
}
