using NCalc;
using System.Text;

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
            var normalizedExpression = NormalizeExpression(expression);
            var expr = new Expression(normalizedExpression);
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

    internal static string NormalizeExpression(string expression)
    {
        if (expression is null)
            return string.Empty;

        var sanitized = RemoveNumericSeparators(expression);
        return sanitized.ReplaceLineEndings(" ").Trim();
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

    private static string RemoveNumericSeparators(string expression)
    {
        var sb = new StringBuilder(expression.Length);
        var inString = false;
        char stringDelimiter = '\0';

        for (var i = 0; i < expression.Length; i++)
        {
            var c = expression[i];

            if (inString)
            {
                sb.Append(c);
                if (c == stringDelimiter && (i == 0 || expression[i - 1] != '\\'))
                    inString = false;
                continue;
            }

            if (c == '"' || c == '\'')
            {
                inString = true;
                stringDelimiter = c;
                sb.Append(c);
                continue;
            }

            if (c == '_' && i > 0 && i < expression.Length - 1 && char.IsDigit(expression[i - 1]) && char.IsDigit(expression[i + 1]))
                continue;

            sb.Append(c);
        }

        return sb.ToString();
    }
}
