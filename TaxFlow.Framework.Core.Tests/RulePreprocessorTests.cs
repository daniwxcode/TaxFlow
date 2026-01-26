using Core.Domain.Tax.Calculation;

using Xunit;

namespace TaxFlow.Framework.Core.Tests;

/// <summary>
/// Unit tests exercising the RulePreprocessor normalization helpers.
/// </summary>
/// <remarks>Validates numeric literal sanitization and ternary expression flattening.</remarks>
public class RulePreprocessorTests
{
    /// <summary>
    /// Ensures numeric separators are removed while emitting a diagnostic entry.
    /// </summary>
    /// <remarks>Asserts both the transformed expression and the recorded diagnostic message.</remarks>
    [Fact]
    public void Process_RemovesNumericSeparators()
    {
        RulePreprocessorResult result = RulePreprocessor.Process("[Base]*1_000_000");

        Assert.Equal("[Base]*1000000", result.Expression);
        Assert.Contains(result.Diagnostics, d => d.Message.Contains("numeric separators", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Verifies multi-line ternary expressions are compacted to a single line for NCalc compatibility.
    /// </summary>
    /// <remarks>Also evaluates the final expression to ensure semantic equivalence.</remarks>
    [Fact]
    public void Process_Normalizes_Multiline_Ternaries()
    {
        string expression = """
        [Value]>0 ?
            1 :
            2
        """;

        RulePreprocessorResult result = RulePreprocessor.Process(expression);

        Assert.DoesNotContain("\n", result.Expression, StringComparison.Ordinal);

        ExpressionEvaluationResult eval = NCalcExpressionEvaluator.Instance.Evaluate(result.Expression, new Dictionary<string, object?> { ["Value"] = 5d });
        Assert.True(eval.IsSuccess, eval.ErrorMessage);
        Assert.Equal(1m, eval.Value);
    }
}
