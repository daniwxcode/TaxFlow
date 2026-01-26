using Core.Domain.Contracts;
using Core.Domain.Enums;
using Core.Domain.Localization;

namespace Core.Domain.Tax.Calculation.Services;

/// <summary>
/// Abstraction for evaluating tax rules.
/// Supports Dependency Inversion Principle and testability.
/// </summary>
public interface ITaxRuleEvaluator
{
    /// <summary>
    /// Evaluates a tax rule with the given attributes.
    /// </summary>
    TaxRuleEvaluationResult Evaluate(
        TaxRule rule,
        IEnumerable<ExtendedAttribute> attributes,
        IReadOnlyCollection<AttributeDefinition> expectedAttributes,
        decimal? amount = null);
}

/// <summary>
/// Default implementation of ITaxRuleEvaluator.
/// </summary>
public sealed class DefaultTaxRuleEvaluator : ITaxRuleEvaluator
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
    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="expressionEvaluator"></param>
    public DefaultTaxRuleEvaluator(IExpressionEvaluator? expressionEvaluator = null)
    {
        _expressionEvaluator = expressionEvaluator ?? NCalcExpressionEvaluator.Instance;
    }

    /// <summary>
    /// Singleton instance with default evaluator.
    /// </summary>
    public static ITaxRuleEvaluator Default { get; } = new DefaultTaxRuleEvaluator();

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
        {
            return TaxRuleEvaluationResult.CreateFailure(string.Empty, ExceptionMessages.RuleCannotBeNull.Format());
        }

        if (string.IsNullOrWhiteSpace(rule.Key))
        {
            return TaxRuleEvaluationResult.CreateFailure(rule.Key ?? string.Empty, ExceptionMessages.RuleKeyMustNotBeEmpty.Format());
        }

        if (!rule.Enabled)
        {
            return TaxRuleEvaluationResult.CreateSuccess(rule.Key, 0m, [ExceptionMessages.RuleDisabled.Format()]);
        }

        Dictionary<string, object?> parameters = BuildParameters(attributes, expectedAttributes, amount);
        ExpressionEvaluationResult evalResult = _expressionEvaluator.Evaluate(rule.Expression, parameters);

        if (!evalResult.IsSuccess)
        {
            return TaxRuleEvaluationResult.CreateFailure(rule.Key, evalResult.ErrorMessage ?? ExceptionMessages.EvaluationFailed.Format());
        }

        var warnings = evalResult.MissingParameters.Count > 0
            ? [ExceptionMessages.MissingParameters.Format(("parameters", string.Join(", ", evalResult.MissingParameters)))]
            : Array.Empty<string>();

        return TaxRuleEvaluationResult.CreateSuccess(rule.Key, evalResult.Value, warnings);
    }

    private Dictionary<string, object?> BuildParameters(
        IEnumerable<ExtendedAttribute> attributes,
        IReadOnlyCollection<AttributeDefinition> expectedAttributes,
        decimal? amount)
    {
        Dictionary<string, object?> parameters = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, AttributeDefinition> expectedByKey = expectedAttributes.ToDictionary(a => a.Key, a => a, StringComparer.OrdinalIgnoreCase);

        if (amount.HasValue)
        {
            parameters[AmountParameterName] = (double)amount.Value;
        }

        foreach (var attr in attributes)
        {
            string? varName = attr.Key?.Trim();
            if (string.IsNullOrWhiteSpace(varName))
            {
                continue;
            }

            if (TryAddEnumParameters(parameters, attr, varName, expectedByKey))
            {
                continue;
            }

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
        {
            return false;
        }

        if (def.DataType != AttributeDataType.Enum || def.EnumDefinition is null)
        {
            return false;
        }

        EnumDefinition enumDef = def.EnumDefinition;

        parameters[varName] = enumDef.TryGetLabel(attr.Value, out var label) ? label : attr.Value;

        if (enumDef.TryGetCode(attr.Value, out var code))
        {
            parameters[$"{varName}{CodeSuffix}"] = code;
        }

        if (enumDef.TryGetLabel(attr.Value, out var lbl))
        {
            parameters[$"{varName}{LabelSuffix}"] = lbl;
        }

        return true;
    }

    private static void AddTypedParameter(Dictionary<string, object?> parameters, ExtendedAttribute attr, string varName)
    => parameters[varName] = double.TryParse(attr.Value, out var num) ? num : bool.TryParse(attr.Value, out var b) ? b : attr.Value;    
}
