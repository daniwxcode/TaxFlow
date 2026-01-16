using Core.Domain.Contracts;
using Core.Domain.Contracts.Validation;
using Core.Domain.Enums;

using System.Text.RegularExpressions;

namespace Core.Domain.Tax.Assets;

/// <summary>
/// Validates extended attributes against attribute definitions.
/// Extracted from AssetType to follow Single Responsibility Principle.
/// </summary>
public sealed class AttributeValidator
{
    /// <summary>
    /// Singleton instance for convenience.
    /// </summary>
    public static AttributeValidator Default { get; } = new();

    /// <summary>
    /// Validates a set of extended attributes against expected attribute definitions.
    /// </summary>
    /// <param name="attributes">The attributes to validate.</param>
    /// <param name="expectedAttributes">The expected attribute definitions.</param>
    /// <returns>A validation result containing any errors.</returns>
    public ValidationResult Validate(
        IEnumerable<ExtendedAttribute> attributes,
        IReadOnlyCollection<AttributeDefinition> expectedAttributes)
    {
        ArgumentNullException.ThrowIfNull(attributes);

        var errors = new List<ValidationError>();
        var attributesByKey = GroupAndValidateDuplicates(attributes, errors);

        foreach (var expected in expectedAttributes)
        {
            ValidateExpectedAttribute(expected, attributesByKey, errors);
        }

        return errors.Count == 0 ? ValidationResult.Success() : ValidationResult.Failure(errors);
    }

    private static Dictionary<string, ExtendedAttribute> GroupAndValidateDuplicates(
        IEnumerable<ExtendedAttribute> attributes,
        List<ValidationError> errors)
    {
        var groups = attributes
            .Where(a => !string.IsNullOrWhiteSpace(a.Key))
            .GroupBy(a => a.Key, StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var group in groups.Where(g => g.Count() > 1))
        {
            errors.Add(new ValidationError(
                ValidationErrorCodes.DuplicateAttribute,
                $"Attribut dupliqué détecté pour la clé '{group.Key}'.",
                group.Key));
        }

        return groups.ToDictionary(
            g => g.Key,
            g => g.OrderByDescending(a => a.ValidFrom).First(),
            StringComparer.OrdinalIgnoreCase);
    }

    private static void ValidateExpectedAttribute(
        AttributeDefinition expected,
        Dictionary<string, ExtendedAttribute> attributesByKey,
        List<ValidationError> errors)
    {
        if (!attributesByKey.TryGetValue(expected.Key, out var provided))
        {
            if (expected.IsRequired)
            {
                errors.Add(new ValidationError(
                    ValidationErrorCodes.MissingRequiredAttribute,
                    $"Attribut requis manquant: '{expected.Key}'.",
                    expected.Key));
            }
            return;
        }

        ValidateDataType(expected, provided, errors);
        ValidateValue(expected, provided, errors);

        if (expected.DataType == AttributeDataType.Enum)
        {
            ValidateEnumValue(expected, provided, errors);
        }
        else
        {
            ValidateRegexPattern(expected, provided, errors);
        }
    }

    private static void ValidateDataType(
        AttributeDefinition expected,
        ExtendedAttribute provided,
        List<ValidationError> errors)
    {
        if (expected.DataType is null)
            return;

        if (provided.DataTypeValue != expected.DataType.Value)
        {
            errors.Add(new ValidationError(
                ValidationErrorCodes.InvalidDataType,
                $"Type invalide pour '{expected.Key}': attendu {expected.DataType}, obtenu {provided.DataType}.",
                expected.Key));
        }
    }

    private static void ValidateValue(
        AttributeDefinition expected,
        ExtendedAttribute provided,
        List<ValidationError> errors)
    {
        if (!provided.IsValidValue())
        {
            errors.Add(new ValidationError(
                ValidationErrorCodes.InvalidValue,
                $"Valeur invalide pour '{expected.Key}' au format {provided.DataType}.",
                expected.Key));
        }
    }

    private static void ValidateEnumValue(
        AttributeDefinition expected,
        ExtendedAttribute provided,
        List<ValidationError> errors)
    {
        var enumDef = expected.EnumDefinition;

        if (enumDef?.Items is null || !enumDef.Items.Any())
        {
            errors.Add(new ValidationError(
                ValidationErrorCodes.MissingEnumDefinition,
                $"Définition d'enum manquante pour l'attribut '{expected.Key}'.",
                expected.Key));
            return;
        }

        var providedValue = (provided.Value ?? string.Empty).Trim();
        var isValidCode = enumDef.TryGetCode(providedValue, out _);
        var isValidLabel = enumDef.TryGetLabel(providedValue, out _);

        if (isValidCode || isValidLabel)
            return;

        var allowedValues = GetAllowedEnumValues(enumDef);
        errors.Add(new ValidationError(
            ValidationErrorCodes.InvalidEnumValue,
            $"Valeur invalide pour '{expected.Key}': '{providedValue}' n'est pas dans les valeurs autorisées [{allowedValues}].",
            expected.Key));
    }

    private static string GetAllowedEnumValues(EnumDefinition enumDef)
    {
        var codes = enumDef.Items
            .Select(i => i.Code)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Select(c => c!.Trim());

        var labels = enumDef.Items
            .Select(i => i.Label)
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l => l!.Trim());

        return string.Join(", ", codes.Concat(labels).Distinct(StringComparer.OrdinalIgnoreCase));
    }

    private static void ValidateRegexPattern(
        AttributeDefinition expected,
        ExtendedAttribute provided,
        List<ValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(expected.RegexPattern))
            return;

        try
        {
            if (!Regex.IsMatch(provided.Value ?? string.Empty, expected.RegexPattern))
            {
                errors.Add(new ValidationError(
                    ValidationErrorCodes.RegexMismatch,
                    $"La valeur de '{expected.Key}' ne respecte pas le motif requis.",
                    expected.Key));
            }
        }
        catch (ArgumentException)
        {
            errors.Add(new ValidationError(
                ValidationErrorCodes.InvalidRegexPattern,
                $"Motif Regex invalide pour la définition d'attribut '{expected.Key}'.",
                expected.Key));
        }
    }
}
