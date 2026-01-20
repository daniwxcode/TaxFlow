using NCalc;

namespace Core.Domain.Tax.Calculation;

/// <summary>
/// NCalc-based implementation of IExpressionEvaluator.
/// </summary>
public sealed class NCalcExpressionEvaluator : IExpressionEvaluator
{
    /// <summary>
    /// Singleton instance for convenience.
    /// </summary>
    public static readonly NCalcExpressionEvaluator Instance = new();

    public ExpressionEvaluationResult Evaluate(string expression, IDictionary<string, object?> parameters)
    {
        if (string.IsNullOrWhiteSpace(expression))
            return ExpressionEvaluationResult.Failure("Expression must not be empty.");

        try
        {
            var expr = new Expression(expression);
            var missingParameters = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Set parameters
            foreach (var kvp in parameters)
            {
                expr.Parameters[kvp.Key] = kvp.Value;
            }

            // Handle missing parameters gracefully
            expr.EvaluateParameter += (name, args) =>
            {
                if (name is string paramKey && !expr.Parameters.ContainsKey(paramKey))
                {
                    missingParameters.Add(paramKey);
                    args.Result = null;
                }
            };

            var result = expr.Evaluate();
            var decimalValue = ConvertToDecimal(result);

            return ExpressionEvaluationResult.Success(decimalValue, missingParameters);
        }
        catch (Exception ex)
        {
            return ExpressionEvaluationResult.Failure(ex.Message);
        }
    }

    private static decimal? ConvertToDecimal(object? result)
    {
        return result switch
        {
            null => null,
            double d => (decimal)d,
            int i => i,
            decimal dec => dec,
            float f => (decimal)f,
            long l => l,
            _ => decimal.TryParse(result.ToString(), out var parsed) ? parsed : null
        };
    }
}
