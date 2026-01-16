namespace Core.Domain.Contracts.Validation;

/// <summary>
/// Standard validation error codes used across the domain.
/// </summary>
public static class ValidationErrorCodes
{
    // Attribute validation
    public const string DuplicateAttribute = "DUPLICATE_ATTRIBUTE";
    public const string MissingRequiredAttribute = "MISSING_REQUIRED_ATTRIBUTE";
    public const string InvalidDataType = "INVALID_DATA_TYPE";
    public const string InvalidValue = "INVALID_VALUE";
    public const string InvalidEnumValue = "INVALID_ENUM_VALUE";
    public const string MissingEnumDefinition = "MISSING_ENUM_DEFINITION";
    public const string InvalidRegexPattern = "INVALID_REGEX_PATTERN";
    public const string RegexMismatch = "REGEX_MISMATCH";

    // Tax rule validation
    public const string RuleNotFound = "RULE_NOT_FOUND";
    public const string RuleDisabled = "RULE_DISABLED";
    public const string RuleEvaluationFailed = "RULE_EVALUATION_FAILED";
    public const string MissingParameters = "MISSING_PARAMETERS";
    public const string EmptyRuleKey = "EMPTY_RULE_KEY";

    // General
    public const string ValidationFailed = "VALIDATION_FAILED";
    public const string NullArgument = "NULL_ARGUMENT";
    public const string EmptyArgument = "EMPTY_ARGUMENT";
}
