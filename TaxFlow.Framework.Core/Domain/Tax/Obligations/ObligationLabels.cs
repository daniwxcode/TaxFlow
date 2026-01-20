using Core.Domain.Localization;

namespace Core.Domain.Tax.Obligations;

/// <summary>
/// Provides localized labels for obligation-related entities.
/// </summary>
public static class ObligationLabels
{
    // ============================================
    // DEADLINE TYPES
    // ============================================

    public static readonly LocalizedString Declaration = LocalizedString.Create("Déclaration")
        .En("Declaration")
        .Ar("?????")
        .Pt("Declaração")
        .Es("Declaración");

    public static readonly LocalizedString Payment = LocalizedString.Create("Paiement")
        .En("Payment")
        .Ar("???")
        .Pt("Pagamento")
        .Es("Pago");

    // ============================================
    // PERIODICITY
    // ============================================

    public static readonly LocalizedString OneTime = LocalizedString.Create("Unique")
        .En("One-time")
        .Ar("??? ?????")
        .Pt("Único")
        .Es("Único");

    public static readonly LocalizedString Monthly = LocalizedString.Create("Mensuel")
        .En("Monthly")
        .Ar("????")
        .Pt("Mensal")
        .Es("Mensual");

    public static readonly LocalizedString Quarterly = LocalizedString.Create("Trimestriel")
        .En("Quarterly")
        .Ar("??? ????")
        .Pt("Trimestral")
        .Es("Trimestral");

    public static readonly LocalizedString SemiAnnual = LocalizedString.Create("Semestriel")
        .En("Semi-annual")
        .Ar("??? ????")
        .Pt("Semestral")
        .Es("Semestral");

    public static readonly LocalizedString Annual = LocalizedString.Create("Annuel")
        .En("Annual")
        .Ar("????")
        .Pt("Anual")
        .Es("Anual");

    public static readonly LocalizedString EventDriven = LocalizedString.Create("Événementiel")
        .En("Event-driven")
        .Ar("??? ?????")
        .Pt("Por evento")
        .Es("Por evento");

    // ============================================
    // TAX REGIMES
    // ============================================

    public static readonly LocalizedString RegimeGeneral = LocalizedString.Create("Régime général")
        .En("General regime")
        .Ar("?????? ?????")
        .Pt("Regime geral")
        .Es("Régimen general");

    public static readonly LocalizedString RegimeSimplified = LocalizedString.Create("Régime simplifié")
        .En("Simplified regime")
        .Ar("?????? ??????")
        .Pt("Regime simplificado")
        .Es("Régimen simplificado");

    public static readonly LocalizedString RegimeReal = LocalizedString.Create("Régime réel")
        .En("Real regime")
        .Ar("?????? ???????")
        .Pt("Regime real")
        .Es("Régimen real");

    public static readonly LocalizedString RegimeMicro = LocalizedString.Create("Régime micro")
        .En("Micro regime")
        .Ar("???? ??????")
        .Pt("Regime micro")
        .Es("Régimen micro");

    public static readonly LocalizedString RegimeConditional = LocalizedString.Create("Régime conditionnel")
        .En("Conditional regime")
        .Ar("???? ?????")
        .Pt("Regime condicional")
        .Es("Régimen condicional");

    public static readonly LocalizedString RegimeExempt = LocalizedString.Create("Exonéré")
        .En("Exempt")
        .Ar("????")
        .Pt("Isento")
        .Es("Exento");

    // ============================================
    // PAYMENT TYPES
    // ============================================

    public static readonly LocalizedString PaymentFull = LocalizedString.Create("Paiement intégral")
        .En("Full payment")
        .Ar("????? ??????")
        .Pt("Pagamento integral")
        .Es("Pago completo");

    public static readonly LocalizedString PaymentAdvance = LocalizedString.Create("Acompte")
        .En("Advance payment")
        .Ar("???? ?????")
        .Pt("Pagamento adiantado")
        .Es("Anticipo");

    public static readonly LocalizedString PaymentInstallment = LocalizedString.Create("Versement échelonné")
        .En("Installment")
        .Ar("???")
        .Pt("Prestação")
        .Es("Cuota");

    public static readonly LocalizedString PaymentBalance = LocalizedString.Create("Solde de régularisation")
        .En("Balance payment")
        .Ar("???? ??????")
        .Pt("Pagamento de saldo")
        .Es("Pago de saldo");

    public static readonly LocalizedString PaymentWithholding = LocalizedString.Create("Retenue à la source")
        .En("Withholding")
        .Ar("?????????")
        .Pt("Retenção na fonte")
        .Es("Retención en la fuente");

    // ============================================
    // DECLARATION TYPES
    // ============================================

    public static readonly LocalizedString DeclarationInitial = LocalizedString.Create("Déclaration initiale")
        .En("Initial declaration")
        .Ar("??????? ??????")
        .Pt("Declaração inicial")
        .Es("Declaración inicial");

    public static readonly LocalizedString DeclarationCorrective = LocalizedString.Create("Déclaration rectificative")
        .En("Corrective declaration")
        .Ar("????? ??????")
        .Pt("Declaração corretiva")
        .Es("Declaración rectificativa");

    public static readonly LocalizedString DeclarationNull = LocalizedString.Create("Déclaration néant")
        .En("Null declaration")
        .Ar("????? ????")
        .Pt("Declaração nula")
        .Es("Declaración nula");

    public static readonly LocalizedString DeclarationProvisional = LocalizedString.Create("Déclaration provisoire")
        .En("Provisional declaration")
        .Ar("????? ????")
        .Pt("Declaração provisória")
        .Es("Declaración provisional");

    public static readonly LocalizedString DeclarationFinal = LocalizedString.Create("Déclaration définitive")
        .En("Final declaration")
        .Ar("??????? ???????")
        .Pt("Declaração definitiva")
        .Es("Declaración definitiva");

    public static readonly LocalizedString DeclarationSupplementary = LocalizedString.Create("Déclaration complémentaire")
        .En("Supplementary declaration")
        .Ar("????? ??????")
        .Pt("Declaração complementar")
        .Es("Declaración complementaria");

    // ============================================
    // LEGAL TEXT TYPES
    // ============================================

    public static readonly LocalizedString LegalLaw = LocalizedString.Create("Loi")
        .En("Law")
        .Ar("?????")
        .Pt("Lei")
        .Es("Ley");

    public static readonly LocalizedString LegalDecree = LocalizedString.Create("Décret")
        .En("Decree")
        .Ar("?????")
        .Pt("Decreto")
        .Es("Decreto");

    public static readonly LocalizedString LegalOrder = LocalizedString.Create("Arrêté")
        .En("Order")
        .Ar("????")
        .Pt("Portaria")
        .Es("Orden");

    public static readonly LocalizedString LegalCircular = LocalizedString.Create("Circulaire")
        .En("Circular")
        .Ar("?????")
        .Pt("Circular")
        .Es("Circular");

    public static readonly LocalizedString LegalInstruction = LocalizedString.Create("Instruction")
        .En("Instruction")
        .Ar("???????")
        .Pt("Instrução")
        .Es("Instrucción");

    public static readonly LocalizedString LegalTaxCode = LocalizedString.Create("Code Général des Impôts")
        .En("Tax Code")
        .Ar("????? ???????")
        .Pt("Código Tributário")
        .Es("Código Tributario");

    public static readonly LocalizedString LegalFinanceLaw = LocalizedString.Create("Loi de Finances")
        .En("Finance Law")
        .Ar("????? ???????")
        .Pt("Lei de Finanças")
        .Es("Ley de Finanzas");

    public static readonly LocalizedString LegalRegulation = LocalizedString.Create("Règlement")
        .En("Regulation")
        .Ar("????")
        .Pt("Regulamento")
        .Es("Reglamento");

    public static readonly LocalizedString LegalConvention = LocalizedString.Create("Convention")
        .En("Convention")
        .Ar("???????")
        .Pt("Convenção")
        .Es("Convenio");

    public static readonly LocalizedString LegalOther = LocalizedString.Create("Texte")
        .En("Text")
        .Ar("??")
        .Pt("Texto")
        .Es("Texto");

    // ============================================
    // PENALTY TYPES
    // ============================================

    public static readonly LocalizedString PenaltyAssiette = LocalizedString.Create("Pénalité d'assiette")
        .En("Assessment penalty")
        .Ar("????? ???????")
        .Pt("Penalidade de lançamento")
        .Es("Penalidad de evaluación");

    public static readonly LocalizedString PenaltyRecouvrement = LocalizedString.Create("Pénalité de recouvrement")
        .En("Collection penalty")
        .Ar("????? ???????")
        .Pt("Penalidade de cobrança")
        .Es("Penalidad de cobro");

    // ============================================
    // EXTENSION METHODS
    // ============================================

    /// <summary>
    /// Gets the localized label for a periodicity.
    /// </summary>
    public static LocalizedString GetLabel(this DeadlinePeriodicity periodicity) => periodicity switch
    {
        DeadlinePeriodicity.OneTime => OneTime,
        DeadlinePeriodicity.Monthly => Monthly,
        DeadlinePeriodicity.Quarterly => Quarterly,
        DeadlinePeriodicity.SemiAnnual => SemiAnnual,
        DeadlinePeriodicity.Annual => Annual,
        DeadlinePeriodicity.EventDriven => EventDriven,
        _ => LocalizedString.Create(periodicity.ToString())
    };

    /// <summary>
    /// Gets the localized label for a tax regime.
    /// </summary>
    public static LocalizedString GetLabel(this TaxRegime regime) => regime switch
    {
        TaxRegime.General => RegimeGeneral,
        TaxRegime.Simplified => RegimeSimplified,
        TaxRegime.Real => RegimeReal,
        TaxRegime.Micro => RegimeMicro,
        TaxRegime.Conditional => RegimeConditional,
        TaxRegime.Exempt => RegimeExempt,
        _ => LocalizedString.Create(regime.ToString())
    };

    /// <summary>
    /// Gets the localized label for a payment type.
    /// </summary>
    public static LocalizedString GetLabel(this PaymentType paymentType) => paymentType switch
    {
        PaymentType.Full => PaymentFull,
        PaymentType.Advance => PaymentAdvance,
        PaymentType.Installment => PaymentInstallment,
        PaymentType.Balance => PaymentBalance,
        PaymentType.Withholding => PaymentWithholding,
        _ => LocalizedString.Create(paymentType.ToString())
    };

    /// <summary>
    /// Gets the localized label for a declaration type.
    /// </summary>
    public static LocalizedString GetLabel(this DeclarationType declarationType) => declarationType switch
    {
        DeclarationType.Initial => DeclarationInitial,
        DeclarationType.Corrective => DeclarationCorrective,
        DeclarationType.Null => DeclarationNull,
        DeclarationType.Provisional => DeclarationProvisional,
        DeclarationType.Final => DeclarationFinal,
        DeclarationType.Supplementary => DeclarationSupplementary,
        _ => LocalizedString.Create(declarationType.ToString())
    };

    /// <summary>
    /// Gets the localized label for a deadline type.
    /// </summary>
    public static LocalizedString GetLabel(this DeadlineType deadlineType) => deadlineType switch
    {
        DeadlineType.Declaration => Declaration,
        DeadlineType.Payment => Payment,
        _ => LocalizedString.Create(deadlineType.ToString())
    };

    /// <summary>
    /// Gets the localized label for a penalty type.
    /// </summary>
    public static LocalizedString GetLabel(this Core.Domain.Tax.Penalties.PenaltyType penaltyType) => penaltyType switch
    {
        Core.Domain.Tax.Penalties.PenaltyType.Assiette => PenaltyAssiette,
        Core.Domain.Tax.Penalties.PenaltyType.Recouvrement => PenaltyRecouvrement,
        _ => LocalizedString.Create(penaltyType.ToString())
    };
}
