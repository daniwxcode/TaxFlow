using Core.Domain.Localization;

namespace Core.Domain.Localization;

/// <summary>
/// Provides localized exception messages for domain operations.
/// </summary>
public static class ExceptionMessages
{
    // ============================================
    // GENERAL VALIDATION MESSAGES
    // ============================================

    public static readonly LocalizedTemplate NameCannotBeEmpty = LocalizedTemplate.Create(
        "Le nom ne doit pas être vide.",
        ("en-US", "The name cannot be empty."),
        ("ar-SA", "?? ???? ?? ???? ????? ??????."),
        ("pt-PT", "O nome não pode estar vazio."));

    public static readonly LocalizedTemplate KeyCannotBeEmpty = LocalizedTemplate.Create(
        "La clé ne doit pas être vide.",
        ("en-US", "The key cannot be empty."),
        ("ar-SA", "?? ???? ?? ???? ??????? ??????."),
        ("pt-PT", "A chave não pode estar vazia."));

    public static readonly LocalizedTemplate LabelCannotBeEmpty = LocalizedTemplate.Create(
        "Le label ne doit pas être vide.",
        ("en-US", "The label cannot be empty."),
        ("ar-SA", "?? ???? ?? ???? ?????? ??????."),
        ("pt-PT", "O rótulo não pode estar vazio."));

    public static readonly LocalizedTemplate ValueCannotBeNull = LocalizedTemplate.Create(
        "La valeur ne peut pas être nulle.",
        ("en-US", "The value cannot be null."),
        ("ar-SA", "?? ???? ?? ???? ?????? ?????."),
        ("pt-PT", "O valor não pode ser nulo."));

    // ============================================
    // ATTRIBUTE MESSAGES
    // ============================================

    public static readonly LocalizedTemplate AttributeKeyCannotBeEmpty = LocalizedTemplate.Create(
        "La clé de l'attribut ne doit pas être vide.",
        ("en-US", "The attribute key cannot be empty."),
        ("ar-SA", "?? ???? ?? ???? ????? ????? ??????."),
        ("pt-PT", "A chave do atributo não pode estar vazia."));

    public static readonly LocalizedTemplate AttributeLabelCannotBeEmpty = LocalizedTemplate.Create(
        "Le label de l'attribut ne doit pas être vide.",
        ("en-US", "The attribute label cannot be empty."),
        ("ar-SA", "?? ???? ?? ???? ???? ????? ??????."),
        ("pt-PT", "O rótulo do atributo não pode estar vazio."));

    public static readonly LocalizedTemplate ExpectedAttributeKeyCannotBeEmpty = LocalizedTemplate.Create(
        "La clé de l'attribut attendu ne doit pas être vide.",
        ("en-US", "The expected attribute key cannot be empty."),
        ("ar-SA", "?? ???? ?? ???? ????? ????? ???????? ??????."),
        ("pt-PT", "A chave do atributo esperado não pode estar vazia."));

    public static readonly LocalizedTemplate ExpectedAttributeAlreadyExists = LocalizedTemplate.Create(
        "L'attribut attendu '{attributeKey}' existe déjà.",
        ("en-US", "The expected attribute '{attributeKey}' already exists."),
        ("ar-SA", "????? ???????? '{attributeKey}' ?????? ??????."),
        ("pt-PT", "O atributo esperado '{attributeKey}' já existe."));

    public static readonly LocalizedTemplate EnumDefinitionCannotBeNull = LocalizedTemplate.Create(
        "La définition d'énumération ne doit pas être nulle.",
        ("en-US", "The enum definition cannot be null."),
        ("ar-SA", "?? ???? ?? ???? ????? ??????? ??????."),
        ("pt-PT", "A definição de enumeração não pode ser nula."));

    public static readonly LocalizedTemplate DuplicateAttributeDetected = LocalizedTemplate.Create(
        "Attribut dupliqué détecté pour la clé '{key}'.",
        ("en-US", "Duplicate attribute detected for key '{key}'."),
        ("ar-SA", "?? ?????? ??? ????? ??????? '{key}'."),
        ("pt-PT", "Atributo duplicado detectado para a chave '{key}'."));

    public static readonly LocalizedTemplate AttributeValidationFailed = LocalizedTemplate.Create(
        "La validation des attributs a échoué: {errorMessage}",
        ("en-US", "Attributes validation failed: {errorMessage}"),
        ("ar-SA", "??? ?????? ?? ??? ??????: {errorMessage}"),
        ("pt-PT", "A validação dos atributos falhou: {errorMessage}"));

    // ============================================
    // TAX RULE MESSAGES
    // ============================================

    public static readonly LocalizedTemplate TaxRuleKeyCannotBeEmpty = LocalizedTemplate.Create(
        "La clé de la règle fiscale ne doit pas être vide.",
        ("en-US", "The tax rule key cannot be empty."),
        ("ar-SA", "?? ???? ?? ???? ????? ??????? ???????? ??????."),
        ("pt-PT", "A chave da regra fiscal não pode estar vazia."));

    public static readonly LocalizedTemplate TaxRuleAlreadyExists = LocalizedTemplate.Create(
        "Une règle fiscale avec la clé '{ruleKey}' existe déjà.",
        ("en-US", "A tax rule with key '{ruleKey}' already exists."),
        ("ar-SA", "???? ?????? ????? ?????? ???????? '{ruleKey}'."),
        ("pt-PT", "Já existe uma regra fiscal com a chave '{ruleKey}'."));

    public static readonly LocalizedTemplate RuleCannotBeNull = LocalizedTemplate.Create(
        "La règle ne peut pas être nulle.",
        ("en-US", "Rule cannot be null."),
        ("ar-SA", "?? ???? ?? ???? ??????? ?????."),
        ("pt-PT", "A regra não pode ser nula."));

    public static readonly LocalizedTemplate RuleKeyMustNotBeEmpty = LocalizedTemplate.Create(
        "La clé de la règle ne doit pas être vide.",
        ("en-US", "Rule key must not be empty."),
        ("ar-SA", "??? ??? ???? ????? ??????? ??????."),
        ("pt-PT", "A chave da regra não pode estar vazia."));

    public static readonly LocalizedTemplate RuleDisabled = LocalizedTemplate.Create(
        "Règle désactivée.",
        ("en-US", "Rule disabled."),
        ("ar-SA", "??????? ?????."),
        ("pt-PT", "Regra desativada."));

    public static readonly LocalizedTemplate RuleEvaluationFailed = LocalizedTemplate.Create(
        "Règle '{ruleKey}': {error}",
        ("en-US", "Rule '{ruleKey}': {error}"),
        ("ar-SA", "??????? '{ruleKey}': {error}"),
        ("pt-PT", "Regra '{ruleKey}': {error}"));

    public static readonly LocalizedTemplate EvaluationFailed = LocalizedTemplate.Create(
        "L'évaluation a échoué.",
        ("en-US", "Evaluation failed."),
        ("ar-SA", "??? ???????."),
        ("pt-PT", "A avaliação falhou."));

    public static readonly LocalizedTemplate MissingParameters = LocalizedTemplate.Create(
        "Paramètres manquants: {parameters}",
        ("en-US", "Missing parameters: {parameters}"),
        ("ar-SA", "?????? ??????: {parameters}"),
        ("pt-PT", "Parâmetros em falta: {parameters}"));

    public static readonly LocalizedTemplate InvalidObligationSchedule = LocalizedTemplate.Create(
        "Calendrier d'obligations invalide: {errorMessage}",
        ("en-US", "Invalid obligation schedule: {errorMessage}"),
        ("ar-SA", "???? ?????????? ??? ????: {errorMessage}"),
        ("pt-PT", "Calendário de obrigações inválido: {errorMessage}"));

    // ============================================
    // ASSET MESSAGES
    // ============================================

    public static readonly LocalizedTemplate AssetTypeMustBeSet = LocalizedTemplate.Create(
        "Le type d'actif doit être défini.",
        ("en-US", "AssetType must be set."),
        ("ar-SA", "??? ????? ??? ?????."),
        ("pt-PT", "O tipo de ativo deve ser definido."));

    public static readonly LocalizedTemplate AssetTypeMustBeSetToEvaluate = LocalizedTemplate.Create(
        "Le type d'actif doit être défini pour évaluer les taxes.",
        ("en-US", "AssetType must be set to evaluate taxes."),
        ("ar-SA", "??? ????? ??? ????? ?????? ???????."),
        ("pt-PT", "O tipo de ativo deve ser definido para avaliar os impostos."));

    // ============================================
    // DEADLINE MESSAGES
    // ============================================

    public static readonly LocalizedTemplate DeclarationDeadlineAlreadyExists = LocalizedTemplate.Create(
        "Une échéance de déclaration avec la clé '{key}' existe déjà.",
        ("en-US", "A declaration deadline with key '{key}' already exists."),
        ("ar-SA", "???? ?????? ???? ????? ???????? '{key}'."),
        ("pt-PT", "Já existe um prazo de declaração com a chave '{key}'."));

    public static readonly LocalizedTemplate PaymentDeadlineAlreadyExists = LocalizedTemplate.Create(
        "Une échéance de paiement avec la clé '{key}' existe déjà.",
        ("en-US", "A payment deadline with key '{key}' already exists."),
        ("ar-SA", "???? ?????? ???? ??? ???????? '{key}'."),
        ("pt-PT", "Já existe um prazo de pagamento com a chave '{key}'."));

    public static readonly LocalizedTemplate FractionOutOfRange = LocalizedTemplate.Create(
        "La fraction doit être comprise entre 0 (exclusif) et 1 (inclusif).",
        ("en-US", "Fraction must be between 0 (exclusive) and 1 (inclusive)."),
        ("ar-SA", "??? ?? ???? ????? ??? 0 (????) ? 1 (????)."),
        ("pt-PT", "A fração deve estar entre 0 (exclusivo) e 1 (inclusivo)."));

    public static readonly LocalizedTemplate OrderMustBePositive = LocalizedTemplate.Create(
        "L'ordre doit être au minimum 1.",
        ("en-US", "Order must be at least 1."),
        ("ar-SA", "??? ?? ???? ??????? 1 ??? ?????."),
        ("pt-PT", "A ordem deve ser pelo menos 1."));

    // ============================================
    // DATE/PERIOD MESSAGES
    // ============================================

    public static readonly LocalizedTemplate EndDateMustBeGreaterOrEqual = LocalizedTemplate.Create(
        "La date de fin doit être supérieure ou égale à la date de début.",
        ("en-US", "The end date must be greater than or equal to the start date."),
        ("ar-SA", "??? ?? ???? ????? ???????? ???? ?? ?? ????? ????? ?????."),
        ("pt-PT", "A data de fim deve ser maior ou igual à data de início."));

    public static readonly LocalizedTemplate DaysInYearMustBePositive = LocalizedTemplate.Create(
        "Le nombre de jours par an doit être supérieur à 0.",
        ("en-US", "daysInYear must be greater than 0."),
        ("ar-SA", "??? ?? ???? ??? ?????? ?? ????? ???? ?? 0."),
        ("pt-PT", "O número de dias por ano deve ser maior que 0."));

    // ============================================
    // HELPER METHODS
    // ============================================

    /// <summary>
    /// Gets the exception message for a given key in the current culture.
    /// </summary>
    public static string Get(LocalizedTemplate template, params (string key, object? value)[] parameters)
        => template.Format(parameters);

    /// <summary>
    /// Gets the exception message for a given key in a specific culture.
    /// </summary>
    public static string Get(LocalizedTemplate template, string? culture, params (string key, object? value)[] parameters)
        => template.Format(culture, parameters);
}
