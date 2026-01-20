using System;
using System.Collections.Generic;
using Core.Domain.Tax.Calculation;
using Xunit;

namespace TaxFlow.Framework.Core.Tests;

public class RulePreprocessorTests
{
    [Fact]
    public void Process_RemovesNumericSeparators()
    {
        var result = RulePreprocessor.Process("[Base]*1_000_000");

        Assert.Equal("[Base]*1000000", result.Expression);
        Assert.Contains(result.Diagnostics, d => d.Message.Contains("numeric separators", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Process_Normalizes_Multiline_Ternaries()
    {
        var expression = """
        [Value]>0 ?
            1 :
            2
        """;

        var result = RulePreprocessor.Process(expression);

        Assert.DoesNotContain("\n", result.Expression, StringComparison.Ordinal);

        var eval = NCalcExpressionEvaluator.Instance.Evaluate(result.Expression, new Dictionary<string, object?> { ["Value"] = 5d });
        Assert.True(eval.IsSuccess, eval.ErrorMessage);
        Assert.Equal(1m, eval.Value);
    }
}
