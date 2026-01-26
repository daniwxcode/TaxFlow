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
    /// <summary>
    /// Gets a value indicating whether the evaluation was successful.
    /// </summary>
    public bool IsSuccess { get; }
    /// <summary>
    /// Gets the evaluated value when successful.
    /// </summary>
    public decimal? Value { get; }
    /// <summary>
    /// Gets the error message when evaluation fails.
    /// </summary>
    public string? ErrorMessage { get; }
    /// <summary>
    /// Gets the list of missing parameters required for evaluation.
    /// </summary>
    public IReadOnlyList<string> MissingParameters { get; }
    /// <summary>
    /// Gets a successful evaluation result.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="missingParameters"></param>
    /// <returns></returns>
    public static ExpressionEvaluationResult Success(decimal? value, IEnumerable<string>? missingParameters = null)
        => new(true, value, null, missingParameters);
    /// <summary>
    /// Gets a failed evaluation result.
    /// </summary>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    public static ExpressionEvaluationResult Failure(string errorMessage)
        => new(false, null, errorMessage, null);
}
