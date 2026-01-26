namespace Core.Domain.Contracts.Validation;

/// <summary>
/// Standard validation error codes used across the domain.
/// </summary>
public static class ValidationErrorCodes
{
    // Attribute validation
    /// <summary>
    /// Code indicating repeated attribute key found where uniqueness is required.
    /// </summary>
    public const string DuplicateAttribute = "DUPLICATE_ATTRIBUTE";
    /// <summary>
    /// Code indicating missing required attribute.
    /// </summary>
    public const string MissingRequiredAttribute = "MISSING_REQUIRED_ATTRIBUTE";
    /// <summary>
    /// Code indicating that the attribute value does not conform to the expected data type.
    /// </summary>
    public const string InvalidDataType = "INVALID_DATA_TYPE";
    /// <summary>
    /// Code indicating that the attribute value is outside the allowed range.
    /// </summary>
    public const string InvalidValue = "INVALID_VALUE";
    /// <summary>
    /// Code indicating that the attribute value is not among the defined enumeration values.
    /// </summary>
    public const string InvalidEnumValue = "INVALID_ENUM_VALUE";
    /// <summary>
    /// Code indicating that the enumeration definition is missing for validation.
    /// </summary>
    public const string MissingEnumDefinition = "MISSING_ENUM_DEFINITION";
    /// <summary>
    /// Represents the error code used to indicate an invalid regular expression pattern.
    /// </summary>
    public const string InvalidRegexPattern = "INVALID_REGEX_PATTERN";
    /// <summary>
    /// Code indicating that the attribute value does not match the specified regular expression.
    /// </summary>
    public const string RegexMismatch = "REGEX_MISMATCH";

    // Tax rule validation
    /// <summary>
    /// Code indicating that the specified tax rule was not found.
    /// </summary>
    public const string RuleNotFound = "RULE_NOT_FOUND";
    /// <summary>
    /// Code indicating that the specified tax rule is disabled.
    /// </summary>
    public const string RuleDisabled = "RULE_DISABLED";
    /// <summary>
    /// Code indicating that the evaluation of the tax rule failed.
    /// </summary>
    public const string RuleEvaluationFailed = "RULE_EVALUATION_FAILED";
    /// <summary>
    /// Code indicating that required parameters for the tax rule are missing.
    /// </summary>
    public const string MissingParameters = "MISSING_PARAMETERS";
    /// <summary>
    /// Code indicating that the tax rule key is empty.
    /// </summary>
    public const string EmptyRuleKey = "EMPTY_RULE_KEY";

    // General
    /// <summary>
    /// Code indicating that validation has failed.
    /// </summary>
    public const string ValidationFailed = "VALIDATION_FAILED";
    /// <summary>
    /// Code indicating that a null argument was provided where it is not allowed.
    /// </summary>
    public const string NullArgument = "NULL_ARGUMENT";
    /// <summary>
    /// Code indicating that an empty argument was provided where it is not allowed.
    /// 
    public const string EmptyArgument = "EMPTY_ARGUMENT";
}
