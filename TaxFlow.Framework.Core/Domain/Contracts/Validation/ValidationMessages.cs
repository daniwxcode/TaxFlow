using Core.Domain.Localization;

namespace Core.Domain.Contracts.Validation;

/// <summary>
/// Provides localized validation error messages.
/// </summary>
public static class ValidationMessages
{
    // ============================================
    // ATTRIBUTE VALIDATION MESSAGES
    // ============================================

    /// <summary>
    /// Message indicating a duplicated attribute.
    /// </summary>
    public static readonly LocalizedTemplate DuplicateAttribute = LocalizedTemplate.Create(
        "L'attribut '{attributeKey}' est en double.",
        ("en-US", "The attribute '{attributeKey}' is duplicated."),
        ("ar-SA", "????? '{attributeKey}' ?????."),
        ("pt-PT", "O atributo '{attributeKey}' está duplicado."));
    /// <summary>
    /// Message indicating a missing required attribute.
    /// </summary>
    public static readonly LocalizedTemplate MissingRequiredAttribute = LocalizedTemplate.Create(
        "L'attribut requis '{attributeKey}' est manquant.",
        ("en-US", "The required attribute '{attributeKey}' is missing."),
        ("ar-SA", "????? ???????? '{attributeKey}' ??????."),
        ("pt-PT", "O atributo obrigatório '{attributeKey}' está em falta."));
    /// <summary>
    /// Message indicating an invalid data type for an attribute.
    /// </summary>
    public static readonly LocalizedTemplate InvalidDataType = LocalizedTemplate.Create(
        "Le type de données de l'attribut '{attributeKey}' est invalide. Attendu: {expectedType}, Reçu: {actualType}.",
        ("en-US", "The data type of attribute '{attributeKey}' is invalid. Expected: {expectedType}, Got: {actualType}."),
        ("ar-SA", "??? ???????? ????? '{attributeKey}' ??? ????. ?????: {expectedType}, ?? ?????? ???: {actualType}."),
        ("pt-PT", "O tipo de dados do atributo '{attributeKey}' é inválido. Esperado: {expectedType}, Recebido: {actualType}."));
    /// <summary>
    /// Message indicating an invalid value for an attribute.
    /// </summary>
    public static readonly LocalizedTemplate InvalidValue = LocalizedTemplate.Create(
        "La valeur de l'attribut '{attributeKey}' est invalide: {reason}.",
        ("en-US", "The value of attribute '{attributeKey}' is invalid: {reason}."),
        ("ar-SA", "???? ????? '{attributeKey}' ??? ?????: {reason}."),
        ("pt-PT", "O valor do atributo '{attributeKey}' é inválido: {reason}."));
    /// <summary>
    /// Message indicating an invalid enum value for an attribute.
    /// </summary>
    public static readonly LocalizedTemplate InvalidEnumValue = LocalizedTemplate.Create(
        "La valeur '{value}' n'est pas valide pour l'énumération '{attributeKey}'. Valeurs acceptées: {validValues}.",
        ("en-US", "The value '{value}' is not valid for enumeration '{attributeKey}'. Valid values: {validValues}."),
        ("ar-SA", "?????? '{value}' ??? ????? ??????? '{attributeKey}'. ????? ???????: {validValues}."),
        ("pt-PT", "O valor '{value}' não é válido para a enumeração '{attributeKey}'. Valores válidos: {validValues}."));
    /// <summary>
    /// Message indicating a missing enum definition for an attribute.
    /// </summary>
    public static readonly LocalizedTemplate MissingEnumDefinition = LocalizedTemplate.Create(
        "La définition d'énumération est manquante pour l'attribut '{attributeKey}'.",
        ("en-US", "The enum definition is missing for attribute '{attributeKey}'."),
        ("ar-SA", "????? ??????? ????? ????? '{attributeKey}'."),
        ("pt-PT", "A definição de enumeração está em falta para o atributo '{attributeKey}'."));
    /// <summary>
    /// Message indicating a regex mismatch for an attribute.
    /// </summary>
    public static readonly LocalizedTemplate RegexMismatch = LocalizedTemplate.Create(
        "La valeur de l'attribut '{attributeKey}' ne correspond pas au format attendu.",
        ("en-US", "The value of attribute '{attributeKey}' does not match the expected format."),
        ("ar-SA", "???? ????? '{attributeKey}' ?? ?????? ?? ??????? ???????."),
        ("pt-PT", "O valor do atributo '{attributeKey}' não corresponde ao formato esperado."));

    // ============================================
    // TAX RULE VALIDATION MESSAGES
    // ============================================
    /// <summary>
    /// Message indicating that a tax rule was not found.
    /// </summary>
    public static readonly LocalizedTemplate RuleNotFound = LocalizedTemplate.Create(
        "La règle fiscale '{ruleKey}' est introuvable.",
        ("en-US", "The tax rule '{ruleKey}' was not found."),
        ("ar-SA", "??????? ???????? '{ruleKey}' ??? ??????."),
        ("pt-PT", "A regra fiscal '{ruleKey}' não foi encontrada."));
    /// <summary>
    /// Message indicating that a tax rule is disabled.
    /// </summary>
    public static readonly LocalizedTemplate RuleDisabled = LocalizedTemplate.Create(
        "La règle fiscale '{ruleKey}' est désactivée.",
        ("en-US", "The tax rule '{ruleKey}' is disabled."),
        ("ar-SA", "??????? ???????? '{ruleKey}' ?????."),
        ("pt-PT", "A regra fiscal '{ruleKey}' está desativada."));
    /// <summary>
    /// Message indicating that a tax rule evaluation failed.
    /// </summary>
    public static readonly LocalizedTemplate RuleEvaluationFailed = LocalizedTemplate.Create(
        "Erreur lors de l'évaluation de la règle '{ruleKey}': {error}.",
        ("en-US", "Error evaluating rule '{ruleKey}': {error}."),
        ("ar-SA", "??? ?? ????? ??????? '{ruleKey}': {error}."),
        ("pt-PT", "Erro ao avaliar a regra '{ruleKey}': {error}."));

    // ============================================
    // OBLIGATION SCHEDULE VALIDATION MESSAGES
    // ============================================

    /// <summary>
    /// Validation message template for duplicate declaration deadline keys.
    /// </summary>
    /// <remarks>
    /// This message is displayed when multiple declaration deadlines in a schedule have the same unique identifier.
    /// </remarks>
    public static readonly LocalizedTemplate DuplicateDeclarationKey = LocalizedTemplate.Create(
        "Plusieurs échéances de déclaration ont la même clé: '{key}'.",
        ("en-US", "Multiple declaration deadlines have the same key: '{key}'."),
        ("ar-SA", "??? ?????? ????? ??? ??? ???????: '{key}'."),
        ("pt-PT", "Vários prazos de declaração têm a mesma chave: '{key}'."));

    /// <summary>
    /// Validation message template for duplicate payment deadline keys.
    /// </summary>
    /// <remarks>
    /// This message is displayed when multiple payment deadlines in a schedule have the same unique identifier.
    /// </remarks>
    public static readonly LocalizedTemplate DuplicatePaymentKey = LocalizedTemplate.Create(
        "Plusieurs échéances de paiement ont la même clé: '{key}'.",
        ("en-US", "Multiple payment deadlines have the same key: '{key}'."),
        ("ar-SA", "??? ?????? ??? ??? ??? ???????: '{key}'."),
        ("pt-PT", "Vários prazos de pagamento têm a mesma chave: '{key}'."));

    /// <summary>
    /// Validation message template for payment fractions exceeding 100%.
    /// </summary>
    /// <remarks>
    /// This message is displayed when the sum of all payment deadline fractions in a schedule exceeds 1.0 (100%).
    /// </remarks>
    public static readonly LocalizedTemplate InvalidFractionTotal = LocalizedTemplate.Create(
        "Le total des fractions de paiement ({total}) dépasse 100%.",
        ("en-US", "The total payment fractions ({total}) exceed 100%."),
        ("ar-SA", "?????? ????? ????? ({total}) ?????? 100%."),
        ("pt-PT", "O total das frações de pagamento ({total}) excede 100%."));

    /// <summary>
    /// Validation message template for declaration deadlines occurring after linked payment deadlines.
    /// </summary>
    /// <remarks>
    /// This message is displayed when a declaration deadline is scheduled after its associated payment deadline,
    /// violating the logical chronological order requirement.
    /// </remarks>
    public static readonly LocalizedTemplate DeclarationAfterPayment = LocalizedTemplate.Create(
        "L'échéance de déclaration '{declarationKey}' doit être antérieure ou égale à l'échéance de paiement '{paymentKey}'.",
        ("en-US", "The declaration deadline '{declarationKey}' must be before or equal to the payment deadline '{paymentKey}'."),
        ("ar-SA", "???? ??????? '{declarationKey}' ??? ?? ???? ??? ?? ????? ???? ????? '{paymentKey}'."),
        ("pt-PT", "O prazo de declaração '{declarationKey}' deve ser anterior ou igual ao prazo de pagamento '{paymentKey}'."));

    /// <summary>
    /// Validation message template for payment deadlines referencing non-existent declarations.
    /// </summary>
    /// <remarks>
    /// This message is displayed when a payment deadline references a declaration deadline that does not exist in the schedule,
    /// indicating a broken referential integrity constraint.
    /// </remarks>
    public static readonly LocalizedTemplate InvalidLinkedDeclaration = LocalizedTemplate.Create(
        "Le paiement '{paymentKey}' fait référence à une déclaration inexistante: '{declarationKey}'.",
        ("en-US", "The payment '{paymentKey}' references a non-existent declaration: '{declarationKey}'."),
        ("ar-SA", "?????? '{paymentKey}' ???? ??? ????? ??? ?????: '{declarationKey}'."),
        ("pt-PT", "O pagamento '{paymentKey}' referencia uma declaração inexistente: '{declarationKey}'."));

    /// <summary>
    /// Validation message template for duplicate order numbers in deadlines.
    /// </summary>
    /// <remarks>
    /// This message is displayed when multiple deadlines of the same type have the same order number,
    /// which would prevent proper sequencing and processing.
    /// </remarks>
    public static readonly LocalizedTemplate DuplicateOrder = LocalizedTemplate.Create(
        "Plusieurs {deadlineType} ont le même ordre: {order}.",
        ("en-US", "Multiple {deadlineType} have the same order: {order}."),
        ("ar-SA", "??? {deadlineType} ??? ??? ???????: {order}."),
        ("pt-PT", "Vários {deadlineType} têm a mesma ordem: {order}."));

    /// <summary>
    /// Validation message template for schedules with multiple deadlines missing legal references.
    /// </summary>
    /// <remarks>
    /// This message is displayed when a schedule has multiple deadlines but lacks the required legal basis
    /// documentation to justify the complex obligation structure.
    /// </remarks>
    public static readonly LocalizedTemplate MissingLegalBasis = LocalizedTemplate.Create(
        "Les échéances multiples nécessitent des références légales explicites.",
        ("en-US", "Multiple deadlines require explicit legal references."),
        ("ar-SA", "???????? ???????? ????? ????? ??????? ?????."),
        ("pt-PT", "Prazos múltiplos requerem referências legais explícitas."));

    /// <summary>
    /// Validation message template for schedules with advance payments but no balance payment.
    /// </summary>
    /// <remarks>
    /// This message is displayed when a tax obligation schedule includes advance payments but lacks
    /// a final balance payment to complete the total tax obligation.
    /// </remarks>
    public static readonly LocalizedTemplate MissingBalancePayment = LocalizedTemplate.Create(
        "Le calendrier contient des acomptes mais aucun solde de régularisation.",
        ("en-US", "The schedule has advance payments but no balance payment."),
        ("ar-SA", "?????? ????? ??? ????? ????? ???? ?? ???? ???? ????."),
        ("pt-PT", "O calendário tem pagamentos adiantados mas nenhum pagamento de saldo."));

    // ============================================
    // PENALTY MESSAGES
    // ============================================

    /// <summary>
    /// Message template for penalty application notifications.
    /// </summary>
    /// <remarks>
    /// This message provides detailed information about applied penalties including type, amount, rate, and base amount.
    /// Used for audit trails and penalty notifications.
    /// </remarks>
    public static readonly LocalizedTemplate PenaltyApplied = LocalizedTemplate.Create(
        "Pénalité de {penaltyType} appliquée: {amount} ({rate} sur {baseAmount}).",
        ("en-US", "Penalty of {penaltyType} applied: {amount} ({rate} on {baseAmount})."),
        ("ar-SA", "?? ????? ????? {penaltyType}: {amount} ({rate} ??? {baseAmount})."),
        ("pt-PT", "Penalidade de {penaltyType} aplicada: {amount} ({rate} sobre {baseAmount})."));

    /// <summary>
    /// Message template for overdue deadline notifications.
    /// </summary>
    /// <remarks>
    /// This message provides information about deadlines that have passed their due date,
    /// including the deadline label and number of days overdue.
    /// </remarks>
    public static readonly LocalizedTemplate DeadlineOverdue = LocalizedTemplate.Create(
        "L'échéance '{deadlineLabel}' est en retard de {daysLate} jour(s).",
        ("en-US", "The deadline '{deadlineLabel}' is {daysLate} day(s) overdue."),
        ("ar-SA", "?????? ??????? '{deadlineLabel}' ????? ?? {daysLate} ???(????)."),
        ("pt-PT", "O prazo '{deadlineLabel}' está atrasado {daysLate} dia(s)."));

    // ============================================
    // HELPER METHODS
    // ============================================

    /// <summary>
    /// Gets the localized name for a deadline type.
    /// </summary>
    public static LocalizedString GetDeadlineTypeName(string type) => type.ToLowerInvariant() switch
    {
        "declaration" or "déclaration" => LocalizedString.Create("échéances de déclaration")
            .En("declaration deadlines")
            .Ar("?????? ???????")
            .Pt("prazos de declaração"),
        "payment" or "paiement" => LocalizedString.Create("échéances de paiement")
            .En("payment deadlines")
            .Ar("?????? ?????")
            .Pt("prazos de pagamento"),
        _ => LocalizedString.Create(type)
    };

    /// <summary>
    /// Gets the localized name for a penalty type.
    /// </summary>
    public static LocalizedString GetPenaltyTypeName(Core.Domain.Tax.Penalties.PenaltyType type) => type switch
    {
        Tax.Penalties.PenaltyType.Assiette => LocalizedString.Create("assiette")
            .En("assessment")
            .Ar("???????")
            .Pt("lançamento"),
        Tax.Penalties.PenaltyType.Recouvrement => LocalizedString.Create("recouvrement")
            .En("collection")
            .Ar("???????")
            .Pt("cobrança"),
        _ => LocalizedString.Create(type.ToString())
    };
}
