namespace Core.Domain.Localization;

/// <summary>
/// Represents a template string with placeholders for localized messages.
/// Supports named parameters like "{amount}" or "{dueDate}".
/// </summary>
public sealed class LocalizedTemplate
{
    private readonly LocalizedString _template;

    /// <summary>
    /// Creates a new localized template.
    /// </summary>
    public LocalizedTemplate(LocalizedString template)
    {
        _template = template ?? throw new ArgumentNullException(nameof(template));
    }

    /// <summary>
    /// Creates a template from a simple string.
    /// </summary>
    public static implicit operator LocalizedTemplate(string template) => 
        new(LocalizedString.Create(template));

    /// <summary>
    /// Creates a template from a localized string.
    /// </summary>
    public static implicit operator LocalizedTemplate(LocalizedString template) => new(template);

    /// <summary>
    /// Creates a new template with the specified default value.
    /// </summary>
    public static LocalizedTemplate Create(string defaultValue) => 
        new(LocalizedString.Create(defaultValue));

    /// <summary>
    /// Creates a template with translations.
    /// </summary>
    public static LocalizedTemplate Create(string defaultValue, params (string culture, string value)[] translations) =>
        new(LocalizedString.Create(defaultValue, translations));

    /// <summary>
    /// Formats the template with the provided parameters.
    /// </summary>
    public string Format(params (string key, object? value)[] parameters)
    {
        return Format(null, parameters);
    }

    /// <summary>
    /// Formats the template with the provided parameters for a specific culture.
    /// </summary>
    public string Format(string? culture, params (string key, object? value)[] parameters)
    {
        var template = _template.GetValue(culture);
        
        foreach (var (key, value) in parameters)
        {
            var placeholder = $"{{{key}}}";
            var replacement = FormatValue(value, culture);
            template = template.Replace(placeholder, replacement, StringComparison.OrdinalIgnoreCase);
        }

        return template;
    }

    /// <summary>
    /// Formats a dictionary of parameters.
    /// </summary>
    public string Format(IDictionary<string, object?> parameters, string? culture = null)
    {
        var tuples = parameters.Select(kvp => (kvp.Key, kvp.Value)).ToArray();
        return Format(culture, tuples);
    }

    private static string FormatValue(object? value, string? culture)
    {
        return value switch
        {
            null => string.Empty,
            LocalizedString localizedString => localizedString.GetValue(culture),
            DateTimeOffset dto => dto.ToString("d", GetCultureInfo(culture)),
            DateTime dt => dt.ToString("d", GetCultureInfo(culture)),
            DateOnly dateOnly => dateOnly.ToString("d", GetCultureInfo(culture)),
            decimal dec => dec.ToString("N2", GetCultureInfo(culture)),
            double dbl => dbl.ToString("N2", GetCultureInfo(culture)),
            _ => value is int i ? i.ToString("N0", GetCultureInfo(culture)) : value.ToString() ?? string.Empty,
        };
    }

    private static System.Globalization.CultureInfo GetCultureInfo(string? culture)
    {
        try
        {
            return System.Globalization.CultureInfo.GetCultureInfo(culture ?? LocalizationContext.CurrentCulture);
        }
        catch
        {
            return System.Globalization.CultureInfo.InvariantCulture;
        }
    }

    /// <summary>
    /// Gets the raw template string.
    /// </summary>
    public LocalizedString Template => _template;
    /// <summary>
    /// To string override returns the default template.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => _template.ToString();
}
