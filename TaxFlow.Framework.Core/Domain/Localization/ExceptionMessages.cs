namespace Core.Domain.Localization;

/// <summary>
/// Provides localized exception messages for domain operations.
/// </summary>
public static class ExceptionMessages
{
    // ============================================
    // GENERAL VALIDATION MESSAGES
    // ============================================
    /// <summary>
    /// Message indicating that the name cannot be empty.
    /// </summary>
    public static readonly LocalizedTemplate NameCannotBeEmpty = LocalizedTemplate.Create(
        "Le nom ne doit pas être vide.",
        ("en-US", "The name cannot be empty."),
        ("ar-SA", "لا يمكن أن يكون الاسم فارغًا."),
        ("pt-PT", "O nome não pode estar vazio."));
    /// <summary>
    /// Message indicating that the key cannot be empty.
    /// </summary>
    public static readonly LocalizedTemplate KeyCannotBeEmpty = LocalizedTemplate.Create(
        "La clé ne doit pas être vide.",
        ("en-US", "The key cannot be empty."),
        ("ar-SA", "لا يمكن أن يكون المفتاح فارغًا."),
        ("pt-PT", "A chave não pode estar vazia."));
    /// <summary>
    /// Message indicating that the label cannot be empty.
    /// </summary>
    public static readonly LocalizedTemplate LabelCannotBeEmpty = LocalizedTemplate.Create(
        "Le label ne doit pas être vide.",
        ("en-US", "The label cannot be empty."),
        ("ar-SA", "لا يمكن أن تكون التسمية فارغة."),
        ("pt-PT", "O rótulo não pode estar vazio."));
    /// <summary>
    /// Message indicating that the value cannot be null.
    /// </summary>
    public static readonly LocalizedTemplate ValueCannotBeNull = LocalizedTemplate.Create(
        "La valeur ne peut pas être nulle.",
        ("en-US", "The value cannot be null."),
        ("ar-SA", "لا يمكن أن تكون القيمة فارغة."),
        ("pt-PT", "O valor não pode ser nulo."));

    // ============================================
    // ATTRIBUTE MESSAGES
    // ============================================
    /// <summary>
    /// Message indicating that the attribute key cannot be empty.
    /// </summary>
    public static readonly LocalizedTemplate AttributeKeyCannotBeEmpty = LocalizedTemplate.Create(
        "La clé de l'attribut ne doit pas être vide.",
        ("en-US", "The attribute key cannot be empty."),
        ("ar-SA", "لا يمكن أن يكون مفتاح السمة فارغًا."),
        ("pt-PT", "A chave do atributo não pode estar vazia."));
    /// <summary>
    /// Message indicating that the attribute label cannot be empty.
    /// </summary>
    public static readonly LocalizedTemplate AttributeLabelCannotBeEmpty = LocalizedTemplate.Create(
        "Le label de l'attribut ne doit pas être vide.",
        ("en-US", "The attribute label cannot be empty."),
        ("ar-SA", "لا يمكن أن تكون تسمية السمة فارغة."),
        ("pt-PT", "O rótulo do atributo não pode estar vazio."));
    /// <summary>
    /// Message indicating that the expected attribute key cannot be empty.
    /// </summary>
    public static readonly LocalizedTemplate ExpectedAttributeKeyCannotBeEmpty = LocalizedTemplate.Create(
        "La clé de l'attribut attendu ne doit pas être vide.",
        ("en-US", "The expected attribute key cannot be empty."),
        ("ar-SA", "لا يمكن أن يكون مفتاح السمة المتوقعة فارغًا."),
        ("pt-PT", "A chave do atributo esperado não pode estar vazia."));
    /// <summary>
    /// Message indicating that the expected attribute already exists.
    /// </summary>
    public static readonly LocalizedTemplate ExpectedAttributeAlreadyExists = LocalizedTemplate.Create(
        "L'attribut attendu '{attributeKey}' existe déjà.",
        ("en-US", "The expected attribute '{attributeKey}' already exists."),
        ("ar-SA", "السمة المتوقعة '{attributeKey}' موجودة بالفعل."),
        ("pt-PT", "O atributo esperado '{attributeKey}' já existe."));

    /// <summary>
    /// Message indicating that the enum definition cannot be null.
    /// </summary>
    public static readonly LocalizedTemplate EnumDefinitionCannotBeNull = LocalizedTemplate.Create(
        "La définition d'énumération ne doit pas être nulle.",
        ("en-US", "The enum definition cannot be null."),
        ("ar-SA", "لا يمكن أن يكون تعريف التعداد فارغًا."),
        ("pt-PT", "A definição de enumeração não pode ser nula."));

    /// <summary>
    /// Message indicating that a duplicate attribute has been detected.
    /// </summary>
    public static readonly LocalizedTemplate DuplicateAttributeDetected = LocalizedTemplate.Create(
        "Attribut dupliqué détecté pour la clé '{key}'.",
        ("en-US", "Duplicate attribute detected for key '{key}'."),
        ("ar-SA", "تم اكتشاف سمة مكررة للمفتاح '{key}'."),
        ("pt-PT", "Atributo duplicado detectado para a chave '{key}'."));

    /// <summary>
    /// Message indicating attribute validation failed.
    /// </summary>
    public static readonly LocalizedTemplate AttributeValidationFailed = LocalizedTemplate.Create(
        "La validation des attributs a échoué: {errorMessage}",
        ("en-US", "Attributes validation failed: {errorMessage}"),
        ("ar-SA", "فشلت عملية التحقق من السمات: {errorMessage}"),
        ("pt-PT", "A validação dos atributos falhou: {errorMessage}"));

    // ============================================
    // TAX RULE MESSAGES
    // ============================================

    /// <summary>
    /// Message indicating that the tax rule key cannot be empty.
    /// </summary>
    public static readonly LocalizedTemplate TaxRuleKeyCannotBeEmpty = LocalizedTemplate.Create(
        "La clé de la règle fiscale ne doit pas être vide.",
        ("en-US", "The tax rule key cannot be empty."),
        ("ar-SA", "لا يمكن أن يكون مفتاح قاعدة الضريبة فارغًا."),
        ("pt-PT", "A chave da regra fiscal não pode estar vazia."));

    /// <summary>
    /// Message indicating that a tax rule already exists.
    /// </summary>
    public static readonly LocalizedTemplate TaxRuleAlreadyExists = LocalizedTemplate.Create(
        "Une règle fiscale avec la clé '{ruleKey}' existe déjà.",
        ("en-US", "A tax rule with key '{ruleKey}' already exists."),
        ("ar-SA", "قاعدة ضريبية بالمفتاح '{ruleKey}' موجودة بالفعل."),
        ("pt-PT", "Já existe uma regra fiscal com a chave '{ruleKey}'."));

    /// <summary>
    /// Message indicating that a rule cannot be null.
    /// </summary>
    public static readonly LocalizedTemplate RuleCannotBeNull = LocalizedTemplate.Create(
        "La règle ne peut pas être nulle.",
        ("en-US", "Rule cannot be null."),
        ("ar-SA", "لا يمكن أن تكون القاعدة فارغة."),
        ("pt-PT", "A regra não pode ser nula."));

    /// <summary>
    /// Message indicating that a rule key must not be empty.
    /// </summary>
    public static readonly LocalizedTemplate RuleKeyMustNotBeEmpty = LocalizedTemplate.Create(
        "La clé de la règle ne doit pas être vide.",
        ("en-US", "Rule key must not be empty."),
        ("ar-SA", "يجب ألا يكون مفتاح القاعدة فارغًا."),
        ("pt-PT", "A chave da regra não pode estar vazia."));

    /// <summary>
    /// Message indicating that a rule is disabled.
    /// </summary>
    public static readonly LocalizedTemplate RuleDisabled = LocalizedTemplate.Create(
        "Règle désactivée.",
        ("en-US", "Rule disabled."),
        ("ar-SA", "تم تعطيل القاعدة."),
        ("pt-PT", "Regra desativada."));

    /// <summary>
    /// Message indicating that rule evaluation failed.
    /// </summary>
    public static readonly LocalizedTemplate RuleEvaluationFailed = LocalizedTemplate.Create(
        "Règle '{ruleKey}': {error}",
        ("en-US", "Rule '{ruleKey}': {error}"),
        ("ar-SA", "القاعدة '{ruleKey}': {error}"),
        ("pt-PT", "Regra '{ruleKey}': {error}"));

    /// <summary>
    /// Message indicating that evaluation failed.
    /// </summary>
    public static readonly LocalizedTemplate EvaluationFailed = LocalizedTemplate.Create(
        "L'évaluation a échoué.",
        ("en-US", "Evaluation failed."),
        ("ar-SA", "فشلت عملية التقييم."),
        ("pt-PT", "A avaliação falhou."));

    /// <summary>
    /// Message indicating that required parameters are missing.
    /// </summary>
    public static readonly LocalizedTemplate MissingParameters = LocalizedTemplate.Create(
        "Paramètres manquants: {parameters}",
        ("en-US", "Missing parameters: {parameters}"),
        ("ar-SA", "المعاملات المفقودة: {parameters}"),
        ("pt-PT", "Parâmetros em falta: {parameters}"));

    /// <summary>
    /// Message indicating that an obligation schedule is invalid.
    /// </summary>
    public static readonly LocalizedTemplate InvalidObligationSchedule = LocalizedTemplate.Create(
        "Calendrier d'obligations invalide: {errorMessage}",
        ("en-US", "Invalid obligation schedule: {errorMessage}"),
        ("ar-SA", "جدول الالتزامات غير صالح: {errorMessage}"),
        ("pt-PT", "Calendário de obrigações inválido: {errorMessage}"));

    /// <summary>
    /// Message indicating that a tax rule expression is invalid.
    /// </summary>
    public static readonly LocalizedTemplate InvalidTaxRuleExpression = LocalizedTemplate.Create(
        "Expression invalide pour la règle '{ruleKey}': {error}",
        ("en-US", "Invalid expression for rule '{ruleKey}': {error}"),
        ("ar-SA", "التعبير غير صالح للقاعدة '{ruleKey}': {error}"),
        ("pt-PT", "Expressão inválida para a regra '{ruleKey}': {error}"));

    // ============================================
    // ASSET MESSAGES
    // ============================================

    /// <summary>
    /// Message indicating that an asset type must be set.
    /// </summary>
    public static readonly LocalizedTemplate AssetTypeMustBeSet = LocalizedTemplate.Create(
        "Le type d'actif doit être défini.",
        ("en-US", "AssetType must be set."),
        ("ar-SA", "يجب تحديد نوع الأصل."),
        ("pt-PT", "O tipo de ativo deve ser definido."));

    /// <summary>
    /// Message indicating that an asset type must be set to evaluate taxes.
    /// </summary>
    public static readonly LocalizedTemplate AssetTypeMustBeSetToEvaluate = LocalizedTemplate.Create(
        "Le type d'actif doit être défini pour évaluer les taxes.",
        ("en-US", "AssetType must be set to evaluate taxes."),
        ("ar-SA", "يجب تحديد نوع الأصل لحساب الضرائب."),
        ("pt-PT", "O tipo de ativo deve ser definido para avaliar os impostos."));

    // ============================================
    // DEADLINE MESSAGES
    // ============================================

    /// <summary>
    /// Message indicating a duplicate declaration deadline.
    /// </summary>
    public static readonly LocalizedTemplate DeclarationDeadlineAlreadyExists = LocalizedTemplate.Create(
        "Une échéance de déclaration avec la clé '{key}' existe déjà.",
        ("en-US", "A declaration deadline with key '{key}' already exists."),
        ("ar-SA", "يوجد بالفعل موعد نهائي للتصريح بالمفتاح '{key}'."),
        ("pt-PT", "Já existe um prazo de declaração com a chave '{key}'."));

    /// <summary>
    /// Message indicating a duplicate payment deadline.
    /// </summary>
    public static readonly LocalizedTemplate PaymentDeadlineAlreadyExists = LocalizedTemplate.Create(
        "Une échéance de paiement avec la clé '{key}' existe déjà.",
        ("en-US", "A payment deadline with key '{key}' already exists."),
        ("ar-SA", "يوجد بالفعل موعد نهائي للدفع بالمفتاح '{key}'."),
        ("pt-PT", "Já existe um prazo de pagamento com a chave '{key}'."));

    /// <summary>
    /// Message indicating that the fraction must be within the allowed range.
    /// </summary>
    public static readonly LocalizedTemplate FractionOutOfRange = LocalizedTemplate.Create(
        "La fraction doit être comprise entre 0 (exclusif) et 1 (inclusif).",
        ("en-US", "Fraction must be between 0 (exclusive) and 1 (inclusive)."),
        ("ar-SA", "يجب أن تكون النسبة بين 0 (حصري) و1 (شامل)."),
        ("pt-PT", "A fração deve estar entre 0 (exclusivo) e 1 (inclusivo)."));

    /// <summary>
    /// Message indicating that order values must be positive.
    /// </summary>
    public static readonly LocalizedTemplate OrderMustBePositive = LocalizedTemplate.Create(
        "L'ordre doit être au minimum 1.",
        ("en-US", "Order must be at least 1."),
        ("ar-SA", "يجب أن يكون الترتيب على الأقل 1."),
        ("pt-PT", "A ordem deve ser pelo menos 1."));

    // ============================================
    // DATE/PERIOD MESSAGES
    // ============================================

    /// <summary>
    /// Message indicating that the end date must be greater than or equal to the start date.
    /// </summary>
    public static readonly LocalizedTemplate EndDateMustBeGreaterOrEqual = LocalizedTemplate.Create(
        "La date de fin doit être supérieure ou égale à la date de début.",
        ("en-US", "The end date must be greater than or equal to the start date."),
        ("ar-SA", "يجب أن يكون تاريخ الانتهاء أكبر من أو مساويًا لتاريخ البدء."),
        ("pt-PT", "A data de fim deve ser maior ou igual à data de início."));

    /// <summary>
    /// Message indicating that days in a year must be positive.
    /// </summary>
    public static readonly LocalizedTemplate DaysInYearMustBePositive = LocalizedTemplate.Create(
        "Le nombre de jours par an doit être supérieur à 0.",
        ("en-US", "daysInYear must be greater than 0."),
        ("ar-SA", "يجب أن يكون عدد الأيام في السنة أكبر من 0."),
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
