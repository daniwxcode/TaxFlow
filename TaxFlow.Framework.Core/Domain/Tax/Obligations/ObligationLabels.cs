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

    /// <summary>
    /// Localized label for declaration deadlines.
    /// </summary>
    public static readonly LocalizedString Declaration = LocalizedString.Create("Déclaration")
        .En("Declaration")
        .Ar("تصريح")
        .Pt("Declaração")
        .Es("Declaración");

    /// <summary>
    /// Localized label for payment deadlines.
    /// </summary>
    public static readonly LocalizedString Payment = LocalizedString.Create("Paiement")
        .En("Payment")
        .Ar("دفع")
        .Pt("Pagamento")
        .Es("Pago");

    // ============================================
    // PERIODICITY
    // ============================================

    /// <summary>
    /// Localized label for one-time periodicity.
    /// </summary>
    public static readonly LocalizedString OneTime = LocalizedString.Create("Unique")
        .En("One-time")
        .Ar("مرة واحدة")
        .Pt("Único")
        .Es("Único");

    /// <summary>
    /// Localized label for monthly periodicity.
    /// </summary>
    public static readonly LocalizedString Monthly = LocalizedString.Create("Mensuel")
        .En("Monthly")
        .Ar("شهري")
        .Pt("Mensal")
        .Es("Mensual");

    /// <summary>
    /// Localized label for quarterly periodicity.
    /// </summary>
    public static readonly LocalizedString Quarterly = LocalizedString.Create("Trimestriel")
        .En("Quarterly")
        .Ar("ربع سنوي")
        .Pt("Trimestral")
        .Es("Trimestral");

    /// <summary>
    /// Localized label for semi-annual periodicity.
    /// </summary>
    public static readonly LocalizedString SemiAnnual = LocalizedString.Create("Semestriel")
        .En("Semi-annual")
        .Ar("نصف سنوي")
        .Pt("Semestral")
        .Es("Semestral");

    /// <summary>
    /// Localized label for annual periodicity.
    /// </summary>
    public static readonly LocalizedString Annual = LocalizedString.Create("Annuel")
        .En("Annual")
        .Ar("سنوي")
        .Pt("Anual")
        .Es("Anual");

    /// <summary>
    /// Localized label for event-driven periodicity.
    /// </summary>
    public static readonly LocalizedString EventDriven = LocalizedString.Create("Événementiel")
        .En("Event-driven")
        .Ar("مرتبط بحدث")
        .Pt("Por evento")
        .Es("Por evento");

    // ============================================
    // TAX REGIMES
    // ============================================

    /// <summary>
    /// Localized label for the general tax regime.
    /// </summary>
    public static readonly LocalizedString RegimeGeneral = LocalizedString.Create("Régime général")
        .En("General regime")
        .Ar("نظام عام")
        .Pt("Regime geral")
        .Es("Régimen general");

    /// <summary>
    /// Localized label for the simplified tax regime.
    /// </summary>
    public static readonly LocalizedString RegimeSimplified = LocalizedString.Create("Régime simplifié")
        .En("Simplified regime")
        .Ar("نظام مبسط")
        .Pt("Regime simplificado")
        .Es("Régimen simplificado");

    /// <summary>
    /// Localized label for the real tax regime.
    /// </summary>
    public static readonly LocalizedString RegimeReal = LocalizedString.Create("Régime réel")
        .En("Real regime")
        .Ar("نظام فعلي")
        .Pt("Regime real")
        .Es("Régimen real");

    /// <summary>
    /// Localized label for the micro tax regime.
    /// </summary>
    public static readonly LocalizedString RegimeMicro = LocalizedString.Create("Régime micro")
        .En("Micro regime")
        .Ar("نظام مصغر")
        .Pt("Regime micro")
        .Es("Régimen micro");

    /// <summary>
    /// Localized label for the conditional tax regime.
    /// </summary>
    public static readonly LocalizedString RegimeConditional = LocalizedString.Create("Régime conditionnel")
        .En("Conditional regime")
        .Ar("نظام مشروط")
        .Pt("Regime condicional")
        .Es("Régimen condicional");

    /// <summary>
    /// Localized label for the exempt tax regime.
    /// </summary>
    public static readonly LocalizedString RegimeExempt = LocalizedString.Create("Exonéré")
        .En("Exempt")
        .Ar("معفى")
        .Pt("Isento")
        .Es("Exento");

    // ============================================
    // PAYMENT TYPES
    // ============================================

    /// <summary>
    /// Localized label for a full payment.
    /// </summary>
    public static readonly LocalizedString PaymentFull = LocalizedString.Create("Paiement intégral")
        .En("Full payment")
        .Ar("دفع كامل")
        .Pt("Pagamento integral")
        .Es("Pago completo");

    /// <summary>
    /// Localized label for an advance payment.
    /// </summary>
    public static readonly LocalizedString PaymentAdvance = LocalizedString.Create("Acompte")
        .En("Advance payment")
        .Ar("دفعة مقدمة")
        .Pt("Pagamento adiantado")
        .Es("Anticipo");

    /// <summary>
    /// Localized label for installment payments.
    /// </summary>
    public static readonly LocalizedString PaymentInstallment = LocalizedString.Create("Versement échelonné")
        .En("Installment")
        .Ar("تقسيط")
        .Pt("Prestação")
        .Es("Cuota");

    /// <summary>
    /// Localized label for balance payments.
    /// </summary>
    public static readonly LocalizedString PaymentBalance = LocalizedString.Create("Solde de régularisation")
        .En("Balance payment")
        .Ar("دفع الرصيد")
        .Pt("Pagamento de saldo")
        .Es("Pago de saldo");

    /// <summary>
    /// Localized label for withholding payments.
    /// </summary>
    public static readonly LocalizedString PaymentWithholding = LocalizedString.Create("Retenue à la source")
        .En("Withholding")
        .Ar("اقتطاع عند المصدر")
        .Pt("Retenção na fonte")
        .Es("Retención en la fuente");

    // ============================================
    // DECLARATION TYPES
    // ============================================

    /// <summary>
    /// Localized label for an initial declaration.
    /// </summary>
    public static readonly LocalizedString DeclarationInitial = LocalizedString.Create("Déclaration initiale")
        .En("Initial declaration")
        .Ar("تصريح أولي")
        .Pt("Declaração inicial")
        .Es("Declaración inicial");

    /// <summary>
    /// Localized label for a corrective declaration.
    /// </summary>
    public static readonly LocalizedString DeclarationCorrective = LocalizedString.Create("Déclaration rectificative")
        .En("Corrective declaration")
        .Ar("تصريح تصحيحي")
        .Pt("Declaração corretiva")
        .Es("Declaración rectificativa");

    /// <summary>
    /// Localized label for a null declaration.
    /// </summary>
    public static readonly LocalizedString DeclarationNull = LocalizedString.Create("Déclaration néant")
        .En("Null declaration")
        .Ar("تصريح صفري")
        .Pt("Declaração nula")
        .Es("Declaración nula");

    /// <summary>
    /// Localized label for a provisional declaration.
    /// </summary>
    public static readonly LocalizedString DeclarationProvisional = LocalizedString.Create("Déclaration provisoire")
        .En("Provisional declaration")
        .Ar("تصريح مؤقت")
        .Pt("Declaração provisória")
        .Es("Declaración provisional");

    /// <summary>
    /// Localized label for a final declaration.
    /// </summary>
    public static readonly LocalizedString DeclarationFinal = LocalizedString.Create("Déclaration définitive")
        .En("Final declaration")
        .Ar("تصريح نهائي")
        .Pt("Declaração definitiva")
        .Es("Declaración definitiva");

    /// <summary>
    /// Localized label for a supplementary declaration.
    /// </summary>
    public static readonly LocalizedString DeclarationSupplementary = LocalizedString.Create("Déclaration complémentaire")
        .En("Supplementary declaration")
        .Ar("تصريح تكميلي")
        .Pt("Declaração complementar")
        .Es("Declaración complementaria");

    // ============================================
    // LEGAL TEXT TYPES
    // ============================================

    /// <summary>
    /// Localized label for legal texts of type law.
    /// </summary>
    public static readonly LocalizedString LegalLaw = LocalizedString.Create("Loi")
        .En("Law")
        .Ar("قانون")
        .Pt("Lei")
        .Es("Ley");

    /// <summary>
    /// Localized label for legal texts of type decree.
    /// </summary>
    public static readonly LocalizedString LegalDecree = LocalizedString.Create("Décret")
        .En("Decree")
        .Ar("مرسوم")
        .Pt("Decreto")
        .Es("Decreto");

    /// <summary>
    /// Localized label for legal texts of type order.
    /// </summary>
    public static readonly LocalizedString LegalOrder = LocalizedString.Create("Arrêté")
        .En("Order")
        .Ar("قرار")
        .Pt("Portaria")
        .Es("Orden");

    /// <summary>
    /// Localized label for legal texts of type circular.
    /// </summary>
    public static readonly LocalizedString LegalCircular = LocalizedString.Create("Circulaire")
        .En("Circular")
        .Ar("منشور")
        .Pt("Circular")
        .Es("Circular");

    /// <summary>
    /// Localized label for legal texts of type instruction.
    /// </summary>
    public static readonly LocalizedString LegalInstruction = LocalizedString.Create("Instruction")
        .En("Instruction")
        .Ar("تعليمات")
        .Pt("Instrução")
        .Es("Instrucción");

    /// <summary>
    /// Localized label for the tax code reference.
    /// </summary>
    public static readonly LocalizedString LegalTaxCode = LocalizedString.Create("Code Général des Impôts")
        .En("Tax Code")
        .Ar("مدونة الضرائب العامة")
        .Pt("Código Tributário")
        .Es("Código Tributario");

    /// <summary>
    /// Localized label for the finance law reference.
    /// </summary>
    public static readonly LocalizedString LegalFinanceLaw = LocalizedString.Create("Loi de Finances")
        .En("Finance Law")
        .Ar("قانون المالية")
        .Pt("Lei de Finanças")
        .Es("Ley de Finanzas");

    /// <summary>
    /// Localized label for regulatory texts.
    /// </summary>
    public static readonly LocalizedString LegalRegulation = LocalizedString.Create("Règlement")
        .En("Regulation")
        .Ar("تنظيم")
        .Pt("Regulamento")
        .Es("Reglamento");

    /// <summary>
    /// Localized label for conventions and agreements.
    /// </summary>
    public static readonly LocalizedString LegalConvention = LocalizedString.Create("Convention")
        .En("Convention")
        .Ar("اتفاقية")
        .Pt("Convenção")
        .Es("Convenio");

    /// <summary>
    /// Localized label for other generic legal texts.
    /// </summary>
    public static readonly LocalizedString LegalOther = LocalizedString.Create("Texte")
        .En("Text")
        .Ar("نص")
        .Pt("Texto")
        .Es("Texto");

    // ============================================
    // PENALTY TYPES
    // ============================================

    /// <summary>
    /// Localized label for assessment penalties.
    /// </summary>
    public static readonly LocalizedString PenaltyAssiette = LocalizedString.Create("Pénalité d'assiette")
        .En("Assessment penalty")
        .Ar("غرامة الأساس")
        .Pt("Penalidade de lançamento")
        .Es("Penalidad de evaluación");

    /// <summary>
    /// Localized label for collection penalties.
    /// </summary>
    public static readonly LocalizedString PenaltyRecouvrement = LocalizedString.Create("Pénalité de recouvrement")
        .En("Collection penalty")
        .Ar("غرامة التحصيل")
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
