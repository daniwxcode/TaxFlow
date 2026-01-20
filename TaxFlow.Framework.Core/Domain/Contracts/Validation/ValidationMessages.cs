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

    public static readonly LocalizedTemplate DuplicateAttribute = LocalizedTemplate.Create(
        "L'attribut '{attributeKey}' est en double.",
        ("en-US", "The attribute '{attributeKey}' is duplicated."),
        ("ar-SA", "????? '{attributeKey}' ?????."),
        ("pt-PT", "O atributo '{attributeKey}' está duplicado."));

    public static readonly LocalizedTemplate MissingRequiredAttribute = LocalizedTemplate.Create(
        "L'attribut requis '{attributeKey}' est manquant.",
        ("en-US", "The required attribute '{attributeKey}' is missing."),
        ("ar-SA", "????? ???????? '{attributeKey}' ??????."),
        ("pt-PT", "O atributo obrigatório '{attributeKey}' está em falta."));

    public static readonly LocalizedTemplate InvalidDataType = LocalizedTemplate.Create(
        "Le type de données de l'attribut '{attributeKey}' est invalide. Attendu: {expectedType}, Reçu: {actualType}.",
        ("en-US", "The data type of attribute '{attributeKey}' is invalid. Expected: {expectedType}, Got: {actualType}."),
        ("ar-SA", "??? ???????? ????? '{attributeKey}' ??? ????. ?????: {expectedType}, ?? ?????? ???: {actualType}."),
        ("pt-PT", "O tipo de dados do atributo '{attributeKey}' é inválido. Esperado: {expectedType}, Recebido: {actualType}."));

    public static readonly LocalizedTemplate InvalidValue = LocalizedTemplate.Create(
        "La valeur de l'attribut '{attributeKey}' est invalide: {reason}.",
        ("en-US", "The value of attribute '{attributeKey}' is invalid: {reason}."),
        ("ar-SA", "???? ????? '{attributeKey}' ??? ?????: {reason}."),
        ("pt-PT", "O valor do atributo '{attributeKey}' é inválido: {reason}."));

    public static readonly LocalizedTemplate InvalidEnumValue = LocalizedTemplate.Create(
        "La valeur '{value}' n'est pas valide pour l'énumération '{attributeKey}'. Valeurs acceptées: {validValues}.",
        ("en-US", "The value '{value}' is not valid for enumeration '{attributeKey}'. Valid values: {validValues}."),
        ("ar-SA", "?????? '{value}' ??? ????? ??????? '{attributeKey}'. ????? ???????: {validValues}."),
        ("pt-PT", "O valor '{value}' não é válido para a enumeração '{attributeKey}'. Valores válidos: {validValues}."));

    public static readonly LocalizedTemplate MissingEnumDefinition = LocalizedTemplate.Create(
        "La définition d'énumération est manquante pour l'attribut '{attributeKey}'.",
        ("en-US", "The enum definition is missing for attribute '{attributeKey}'."),
        ("ar-SA", "????? ??????? ????? ????? '{attributeKey}'."),
        ("pt-PT", "A definição de enumeração está em falta para o atributo '{attributeKey}'."));

    public static readonly LocalizedTemplate RegexMismatch = LocalizedTemplate.Create(
        "La valeur de l'attribut '{attributeKey}' ne correspond pas au format attendu.",
        ("en-US", "The value of attribute '{attributeKey}' does not match the expected format."),
        ("ar-SA", "???? ????? '{attributeKey}' ?? ?????? ?? ??????? ???????."),
        ("pt-PT", "O valor do atributo '{attributeKey}' não corresponde ao formato esperado."));

    // ============================================
    // TAX RULE VALIDATION MESSAGES
    // ============================================

    public static readonly LocalizedTemplate RuleNotFound = LocalizedTemplate.Create(
        "La règle fiscale '{ruleKey}' est introuvable.",
        ("en-US", "The tax rule '{ruleKey}' was not found."),
        ("ar-SA", "??????? ???????? '{ruleKey}' ??? ??????."),
        ("pt-PT", "A regra fiscal '{ruleKey}' não foi encontrada."));

    public static readonly LocalizedTemplate RuleDisabled = LocalizedTemplate.Create(
        "La règle fiscale '{ruleKey}' est désactivée.",
        ("en-US", "The tax rule '{ruleKey}' is disabled."),
        ("ar-SA", "??????? ???????? '{ruleKey}' ?????."),
        ("pt-PT", "A regra fiscal '{ruleKey}' está desativada."));

    public static readonly LocalizedTemplate RuleEvaluationFailed = LocalizedTemplate.Create(
        "Erreur lors de l'évaluation de la règle '{ruleKey}': {error}.",
        ("en-US", "Error evaluating rule '{ruleKey}': {error}."),
        ("ar-SA", "??? ?? ????? ??????? '{ruleKey}': {error}."),
        ("pt-PT", "Erro ao avaliar a regra '{ruleKey}': {error}."));

    // ============================================
    // OBLIGATION SCHEDULE VALIDATION MESSAGES
    // ============================================

    public static readonly LocalizedTemplate DuplicateDeclarationKey = LocalizedTemplate.Create(
        "Plusieurs échéances de déclaration ont la même clé: '{key}'.",
        ("en-US", "Multiple declaration deadlines have the same key: '{key}'."),
        ("ar-SA", "??? ?????? ????? ??? ??? ???????: '{key}'."),
        ("pt-PT", "Vários prazos de declaração têm a mesma chave: '{key}'."));

    public static readonly LocalizedTemplate DuplicatePaymentKey = LocalizedTemplate.Create(
        "Plusieurs échéances de paiement ont la même clé: '{key}'.",
        ("en-US", "Multiple payment deadlines have the same key: '{key}'."),
        ("ar-SA", "??? ?????? ??? ??? ??? ???????: '{key}'."),
        ("pt-PT", "Vários prazos de pagamento têm a mesma chave: '{key}'."));

    public static readonly LocalizedTemplate InvalidFractionTotal = LocalizedTemplate.Create(
        "Le total des fractions de paiement ({total}) dépasse 100%.",
        ("en-US", "The total payment fractions ({total}) exceed 100%."),
        ("ar-SA", "?????? ????? ????? ({total}) ?????? 100%."),
        ("pt-PT", "O total das frações de pagamento ({total}) excede 100%."));

    public static readonly LocalizedTemplate DeclarationAfterPayment = LocalizedTemplate.Create(
        "L'échéance de déclaration '{declarationKey}' doit être antérieure ou égale à l'échéance de paiement '{paymentKey}'.",
        ("en-US", "The declaration deadline '{declarationKey}' must be before or equal to the payment deadline '{paymentKey}'."),
        ("ar-SA", "???? ??????? '{declarationKey}' ??? ?? ???? ??? ?? ????? ???? ????? '{paymentKey}'."),
        ("pt-PT", "O prazo de declaração '{declarationKey}' deve ser anterior ou igual ao prazo de pagamento '{paymentKey}'."));

    public static readonly LocalizedTemplate InvalidLinkedDeclaration = LocalizedTemplate.Create(
        "Le paiement '{paymentKey}' fait référence à une déclaration inexistante: '{declarationKey}'.",
        ("en-US", "The payment '{paymentKey}' references a non-existent declaration: '{declarationKey}'."),
        ("ar-SA", "?????? '{paymentKey}' ???? ??? ????? ??? ?????: '{declarationKey}'."),
        ("pt-PT", "O pagamento '{paymentKey}' referencia uma declaração inexistente: '{declarationKey}'."));

    public static readonly LocalizedTemplate DuplicateOrder = LocalizedTemplate.Create(
        "Plusieurs {deadlineType} ont le même ordre: {order}.",
        ("en-US", "Multiple {deadlineType} have the same order: {order}."),
        ("ar-SA", "??? {deadlineType} ??? ??? ???????: {order}."),
        ("pt-PT", "Vários {deadlineType} têm a mesma ordem: {order}."));

    public static readonly LocalizedTemplate MissingLegalBasis = LocalizedTemplate.Create(
        "Les échéances multiples nécessitent des références légales explicites.",
        ("en-US", "Multiple deadlines require explicit legal references."),
        ("ar-SA", "???????? ???????? ????? ????? ??????? ?????."),
        ("pt-PT", "Prazos múltiplos requerem referências legais explícitas."));

    public static readonly LocalizedTemplate MissingBalancePayment = LocalizedTemplate.Create(
        "Le calendrier contient des acomptes mais aucun solde de régularisation.",
        ("en-US", "The schedule has advance payments but no balance payment."),
        ("ar-SA", "?????? ????? ??? ????? ????? ???? ?? ???? ???? ????."),
        ("pt-PT", "O calendário tem pagamentos adiantados mas nenhum pagamento de saldo."));

    // ============================================
    // PENALTY MESSAGES
    // ============================================

    public static readonly LocalizedTemplate PenaltyApplied = LocalizedTemplate.Create(
        "Pénalité de {penaltyType} appliquée: {amount} ({rate} sur {baseAmount}).",
        ("en-US", "Penalty of {penaltyType} applied: {amount} ({rate} on {baseAmount})."),
        ("ar-SA", "?? ????? ????? {penaltyType}: {amount} ({rate} ??? {baseAmount})."),
        ("pt-PT", "Penalidade de {penaltyType} aplicada: {amount} ({rate} sobre {baseAmount})."));

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
