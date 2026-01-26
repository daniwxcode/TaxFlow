namespace Core.Domain.Contracts.Validation;

/// <summary>
/// Represents a single validation error with code, message and optional property context.
/// </summary>
public sealed record ValidationError
{
    /// <summary>
    /// Creates a new validation error.
    /// </summary>
    /// <param name="code">Error code for programmatic handling.</param>
    /// <param name="message">Human-readable error message.</param>
    /// <param name="propertyName">Optional property name that caused the error.</param>
    public ValidationError(string code, string message, string? propertyName = null)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Message = message ?? throw new ArgumentNullException(nameof(message));
        PropertyName = propertyName;
    }

    /// <summary>
    /// Error code for programmatic handling.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Human-readable error message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Optional property name that caused the error.
    /// </summary>
    public string? PropertyName { get; }

    /// <summary>
    /// Returns a string representation of the validation error.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => PropertyName is null
        ? $"[{Code}] {Message}"
        : $"[{Code}] {PropertyName}: {Message}";
}
