namespace Core.Domain.Tax.Calculation;

/// <summary>
/// Abstraction for expression evaluation to decouple from specific implementations like NCalc.
/// </summary>
public interface IExpressionEvaluator
{
    /// <summary>
    /// Evaluates an expression with the given parameters.
    /// </summary>
    /// <param name="expression">The expression string to evaluate.</param>
    /// <param name="parameters">Dictionary of parameter names and values.</param>
    /// <returns>The evaluation result.</returns>
    ExpressionEvaluationResult Evaluate(string expression, IDictionary<string, object?> parameters);
}

/// <summary>
/// Result of expression evaluation.
/// </summary>
public sealed class ExpressionEvaluationResult
{
    private ExpressionEvaluationResult(bool isSuccess, decimal? value, string? errorMessage, IEnumerable<string>? missingParameters)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorMessage = errorMessage;
        MissingParameters = missingParameters?.ToList().AsReadOnly() ?? (IReadOnlyList<string>)[];
    }

    public bool IsSuccess { get; }
    public decimal? Value { get; }
    public string? ErrorMessage { get; }
    public IReadOnlyList<string> MissingParameters { get; }

    public static ExpressionEvaluationResult Success(decimal? value, IEnumerable<string>? missingParameters = null)
        => new(true, value, null, missingParameters);

    public static ExpressionEvaluationResult Failure(string errorMessage)
        => new(false, null, errorMessage, null);
}
