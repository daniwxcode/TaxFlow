namespace Core.Domain.Contracts.Validation;

/// <summary>
/// Represents the result of a validation operation with structured errors.
/// </summary>
public sealed class ValidationResult
{
    private readonly List<ValidationError> _errors;

    private ValidationResult(IEnumerable<ValidationError>? errors = null)
    {
        _errors = errors?.ToList() ?? [];
    }

    /// <summary>
    /// Gets whether the validation succeeded (no errors).
    /// </summary>
    public bool IsValid => _errors.Count == 0;

    /// <summary>
    /// Gets whether the validation failed (has errors).
    /// </summary>
    public bool HasErrors => _errors.Count > 0;

    /// <summary>
    /// Gets the collection of validation errors.
    /// </summary>
    public IReadOnlyList<ValidationError> Errors => _errors.AsReadOnly();

    /// <summary>
    /// Gets all error messages concatenated.
    /// </summary>
    public string ErrorMessage => string.Join("; ", _errors.Select(e => e.Message));

    /// <summary>
    /// Creates a successful validation result.
    /// </summary>
    public static ValidationResult Success() => new();

    /// <summary>
    /// Creates a failed validation result with the specified errors.
    /// </summary>
    public static ValidationResult Failure(IEnumerable<ValidationError> errors) => new(errors);

    /// <summary>
    /// Creates a failed validation result with a single error.
    /// </summary>
    public static ValidationResult Failure(ValidationError error) => new([error]);

    /// <summary>
    /// Creates a failed validation result with a single error message.
    /// </summary>
    public static ValidationResult Failure(string errorCode, string message, string? propertyName = null)
        => new([new ValidationError(errorCode, message, propertyName)]);

    /// <summary>
    /// Combines multiple validation results into one.
    /// </summary>
    public static ValidationResult Combine(params ValidationResult[] results)
    {
        var allErrors = results.SelectMany(r => r.Errors).ToList();
        return allErrors.Count == 0 ? Success() : Failure(allErrors);
    }

    /// <summary>
    /// Implicitly converts error messages to ValidationResult for backward compatibility.
    /// </summary>
    public static ValidationResult FromMessages(IEnumerable<string> messages)
    {
        var errors = messages.Select(m => new ValidationError("VALIDATION_ERROR", m)).ToList();
        return errors.Count == 0 ? Success() : Failure(errors);
    }

    /// <summary>
    /// Gets error messages for backward compatibility.
    /// </summary>
    public IEnumerable<string> ToMessages() => _errors.Select(e => e.Message);
}
