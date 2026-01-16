using Core.Domain.Contracts;
using Core.Domain.Enums;

namespace Core.Domain.Tax.Calculation;

/// <summary>
/// Evaluates tax rules using expression evaluation.
/// Extracted from AssetType to follow Single Responsibility Principle.
/// </summary>
public sealed class TaxRuleEvaluator
{
    private readonly IExpressionEvaluator _expressionEvaluator;

    /// <summary>
    /// Parameter name for the base amount variable.
    /// </summary>
    public const string AmountParameterName = "amount";

    /// <summary>
    /// Suffix for enum code parameters.
    /// </summary>
    public const string CodeSuffix = "Code";

    /// <summary>
    /// Suffix for enum label parameters.
    /// </summary>
    public const string LabelSuffix = "Label";

    public TaxRuleEvaluator(IExpressionEvaluator? expressionEvaluator = null)
    {
        _expressionEvaluator = expressionEvaluator ?? NCalcExpressionEvaluator.Instance;
    }

    /// <summary>
    /// Singleton instance with default evaluator.
    /// </summary>
    public static TaxRuleEvaluator Default { get; } = new();

    /// <summary>
    /// Evaluates a tax rule with the given attributes.
    /// </summary>
    public TaxRuleEvaluationResult Evaluate(
        TaxRule rule,
        IEnumerable<ExtendedAttribute> attributes,
        IReadOnlyCollection<AttributeDefinition> expectedAttributes,
        decimal? amount = null)
    {
        if (rule is null)
            return TaxRuleEvaluationResult.CreateFailure(string.Empty, "Rule cannot be null.");

        if (string.IsNullOrWhiteSpace(rule.Key))
            return TaxRuleEvaluationResult.CreateFailure(rule.Key ?? string.Empty, "Rule key must not be empty.");

        if (!rule.Enabled)
            return TaxRuleEvaluationResult.CreateSuccess(rule.Key, 0m, ["Rule disabled."]);

        var parameters = BuildParameters(attributes, expectedAttributes, amount);
        var evalResult = _expressionEvaluator.Evaluate(rule.Expression, parameters);

        if (!evalResult.IsSuccess)
            return TaxRuleEvaluationResult.CreateFailure(rule.Key, evalResult.ErrorMessage ?? "Evaluation failed.");

        var warnings = evalResult.MissingParameters.Count > 0
            ? [$"Missing parameters: {string.Join(", ", evalResult.MissingParameters)}"]
            : Array.Empty<string>();

        return TaxRuleEvaluationResult.CreateSuccess(rule.Key, evalResult.Value, warnings);
    }

    private Dictionary<string, object?> BuildParameters(
        IEnumerable<ExtendedAttribute> attributes,
        IReadOnlyCollection<AttributeDefinition> expectedAttributes,
        decimal? amount)
    {
        var parameters = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        var expectedByKey = expectedAttributes.ToDictionary(a => a.Key, a => a, StringComparer.OrdinalIgnoreCase);

        if (amount.HasValue)
            parameters[AmountParameterName] = (double)amount.Value;

        foreach (var attr in attributes)
        {
            var varName = attr.Key?.Trim();
            if (string.IsNullOrWhiteSpace(varName))
                continue;

            if (TryAddEnumParameters(parameters, attr, varName, expectedByKey))
                continue;

            AddTypedParameter(parameters, attr, varName);
        }

        return parameters;
    }

    private static bool TryAddEnumParameters(
        Dictionary<string, object?> parameters,
        ExtendedAttribute attr,
        string varName,
        Dictionary<string, AttributeDefinition> expectedByKey)
    {
        if (!expectedByKey.TryGetValue(varName, out var def))
            return false;

        if (def.DataType != AttributeDataType.Enum || def.EnumDefinition is null)
            return false;

        var enumDef = def.EnumDefinition;

        // Set main parameter to label or raw value
        parameters[varName] = enumDef.TryGetLabel(attr.Value, out var label) ? label : attr.Value;

        // Add Code and Label suffixed parameters
        if (enumDef.TryGetCode(attr.Value, out var code))
            parameters[$"{varName}{CodeSuffix}"] = code;

        if (enumDef.TryGetLabel(attr.Value, out var lbl))
            parameters[$"{varName}{LabelSuffix}"] = lbl;

        return true;
    }

    private static void AddTypedParameter(Dictionary<string, object?> parameters, ExtendedAttribute attr, string varName)
    {
        if (double.TryParse(attr.Value, out var num))
            parameters[varName] = num;
        else if (bool.TryParse(attr.Value, out var b))
            parameters[varName] = b;
        else
            parameters[varName] = attr.Value;
    }
}
