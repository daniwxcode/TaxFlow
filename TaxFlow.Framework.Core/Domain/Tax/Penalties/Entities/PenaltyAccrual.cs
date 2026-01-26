namespace Core.Domain.Tax.Penalties;

/// <summary>
/// Represents a single penalty accrual entry.
/// </summary>
public sealed class PenaltyAccrual
{
    /// <summary>
    /// Create a penalty accrual entry.
    /// </summary>
    /// <param name="type">Penalty type.</param>
    /// <param name="lineType">Penalty line classification.</param>
    /// <param name="declarationId">Declaration identifier.</param>
    /// <param name="liquidationId">Liquidation identifier (optional).</param>
    /// <param name="installmentId">Installment identifier (optional).</param>
    /// <param name="baseAmount">Base amount for the penalty.</param>
    /// <param name="amount">Penalty amount.</param>
    /// <param name="rate">Applied rate.</param>
    /// <param name="daysLate">Late days count.</param>
    /// <param name="periodIndex">Period index for periodic penalties (1-based).</param>
    /// <param name="periodDays">Days in the penalty period.</param>
    /// <param name="periodStart">Period start date.</param>
    /// <param name="periodEnd">Period end date.</param>
    /// <param name="asOf">Calculation date.</param>
    public PenaltyAccrual(
        PenaltyType type,
        PenaltyLineType lineType,
        Guid declarationId,
        Guid? liquidationId,
        Guid? installmentId,
        decimal baseAmount,
        decimal amount,
        decimal rate,
        int daysLate,
        int periodIndex,
        int periodDays,
        DateTimeOffset periodStart,
        DateTimeOffset periodEnd,
        DateTimeOffset asOf)
    {
        Type = type;
        LineType = lineType;
        DeclarationId = declarationId;
        LiquidationId = liquidationId;
        InstallmentId = installmentId;
        BaseAmount = baseAmount;
        Amount = amount;
        Rate = rate;
        DaysLate = daysLate;
        PeriodIndex = periodIndex;
        PeriodDays = periodDays;
        PeriodStart = periodStart;
        PeriodEnd = periodEnd;
        AsOf = asOf;
    }

    /// <summary>
    /// Penalty type.
    /// </summary>
    public PenaltyType Type { get; }

    /// <summary>
    /// Penalty line classification.
    /// </summary>
    public PenaltyLineType LineType { get; }

    /// <summary>
    /// Declaration identifier.
    /// </summary>
    public Guid DeclarationId { get; }

    /// <summary>
    /// Liquidation identifier (optional).
    /// </summary>
    public Guid? LiquidationId { get; }

    /// <summary>
    /// Installment identifier (optional).
    /// </summary>
    public Guid? InstallmentId { get; }

    /// <summary>
    /// Base amount for penalty calculation.
    /// </summary>
    public decimal BaseAmount { get; }

    /// <summary>
    /// Penalty amount.
    /// </summary>
    public decimal Amount { get; }

    /// <summary>
    /// Applied rate.
    /// </summary>
    public decimal Rate { get; }

    /// <summary>
    /// Number of days late.
    /// </summary>
    public int DaysLate { get; }

    /// <summary>
    /// Period index for periodic penalties (1-based).
    /// </summary>
    public int PeriodIndex { get; }

    /// <summary>
    /// Days in the penalty period.
    /// </summary>
    public int PeriodDays { get; }

    /// <summary>
    /// Period start date.
    /// </summary>
    public DateTimeOffset PeriodStart { get; }

    /// <summary>
    /// Period end date.
    /// </summary>
    public DateTimeOffset PeriodEnd { get; }

    /// <summary>
    /// Calculation date.
    /// </summary>
    public DateTimeOffset AsOf { get; }
}
