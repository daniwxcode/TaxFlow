using System;
using Core.Domain.Localization;
using NCalc;

namespace Core.Domain.Tax.Calculation;

/// <summary>
/// Provides guard clauses to validate rule expressions before registration.
/// </summary>
internal static class TaxRuleExpressionValidator
{
    public static void Validate(TaxRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);

        if (string.IsNullOrWhiteSpace(rule.Expression))
            throw new ArgumentException(ExceptionMessages.ValueCannotBeNull.Format(), nameof(rule.Expression));

        var normalized = NCalcExpressionEvaluator.NormalizeExpression(rule.Expression);

        var expr = new Expression(normalized);
        if (expr.HasErrors())
        {
            var errorMessage = expr.Error?.Message ?? string.Empty;
            throw new ArgumentException(
                ExceptionMessages.InvalidTaxRuleExpression.Format(("ruleKey", rule.Key ?? string.Empty), ("error", errorMessage)));
        }
    }
}
