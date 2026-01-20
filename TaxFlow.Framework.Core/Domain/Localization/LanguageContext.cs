namespace Core.Domain.Localization;

/// <summary>
/// Provides ambient context for the current localization culture.
/// </summary>
public static class LocalizationContext
{
    private static readonly AsyncLocal<string?> _currentCulture = new();

    /// <summary>
    /// Default culture (French).
    /// </summary>
    public const string DefaultCulture = "fr-FR";

    /// <summary>
    /// Gets or sets the current culture for the ambient context.
    /// </summary>
    public static string CurrentCulture
    {
        get => _currentCulture.Value ?? DefaultCulture;
        set => _currentCulture.Value = value;
    }

    /// <summary>
    /// Sets the culture for a scope, then restores the previous culture.
    /// </summary>
    public static IDisposable WithCulture(string culture)
    {
        var previous = _currentCulture.Value;
        _currentCulture.Value = culture;
        return new CultureScope(previous);
    }

    private sealed class CultureScope(string? previousCulture) : IDisposable
    {
        public void Dispose() => _currentCulture.Value = previousCulture;
    }
}
