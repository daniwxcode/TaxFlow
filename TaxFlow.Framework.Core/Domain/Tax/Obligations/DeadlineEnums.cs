namespace Core.Domain.Tax.Obligations;

/// <summary>
/// Periodicity of a tax obligation deadline.
/// </summary>
public enum DeadlinePeriodicity
{
    /// <summary>
    /// One-time deadline (e.g., upon acquisition).
    /// </summary>
    OneTime = 0,

    /// <summary>
    /// Monthly deadline.
    /// </summary>
    Monthly = 1,

    /// <summary>
    /// Quarterly deadline.
    /// </summary>
    Quarterly = 2,

    /// <summary>
    /// Semi-annual deadline (twice per year).
    /// </summary>
    SemiAnnual = 3,

    /// <summary>
    /// Annual deadline.
    /// </summary>
    Annual = 4,

    /// <summary>
    /// Event-driven deadline (triggered by a specific event).
    /// </summary>
    EventDriven = 5
}

/// <summary>
/// Tax regime/application mode for a deadline.
/// </summary>
public enum TaxRegime
{
    /// <summary>
    /// General regime - applies to all taxpayers by default.
    /// </summary>
    General = 0,

    /// <summary>
    /// Simplified regime - reduced requirements for eligible taxpayers.
    /// </summary>
    Simplified = 1,

    /// <summary>
    /// Real regime - full accounting requirements.
    /// </summary>
    Real = 2,

    /// <summary>
    /// Micro regime - for very small businesses.
    /// </summary>
    Micro = 3,

    /// <summary>
    /// Conditional regime - applies based on specific conditions.
    /// </summary>
    Conditional = 4,

    /// <summary>
    /// Exempt regime - taxpayer is exempt from this obligation.
    /// </summary>
    Exempt = 5
}

/// <summary>
/// Payment type for payment deadlines.
/// </summary>
public enum PaymentType
{
    /// <summary>
    /// Full payment of the tax due.
    /// </summary>
    Full = 0,

    /// <summary>
    /// Advance payment (acompte provisionnel).
    /// </summary>
    Advance = 1,

    /// <summary>
    /// Installment payment (fractionnement).
    /// </summary>
    Installment = 2,

    /// <summary>
    /// Balance payment (solde de régularisation).
    /// </summary>
    Balance = 3,

    /// <summary>
    /// Withholding payment (retenue à la source).
    /// </summary>
    Withholding = 4
}
