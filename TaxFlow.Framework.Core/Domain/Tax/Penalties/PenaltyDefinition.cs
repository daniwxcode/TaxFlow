using System;

namespace Core.Domain.Tax.Penalties;

/// <summary>
/// Defines parameters for a penalty type.
/// </summary>
public sealed class PenaltyDefinition
{
    /// <summary>
    /// Penalty type.
    /// </summary>
    public PenaltyType Type { get; init; }

    /// <summary>
    /// Event that triggers this penalty.
    /// </summary>
    public PenaltyTriggerEvent TriggerEvent { get; init; } = PenaltyTriggerEvent.Any;

    /// <summary>
    /// Fixed penalty amount (applied once).
    /// </summary>
    public decimal FixedAmount { get; init; } = 0m;

    /// <summary>
    /// Grace period in days before penalties start.
    /// </summary>
    public int GraceDays { get; init; } = 0;

    /// <summary>
    /// Periodicity in days for penalties (default 30).
    /// </summary>
    public int PeriodDays { get; init; } = 30;

    /// <summary>
    /// Annual rate for penalties.
    /// </summary>
    public decimal AnnualRate { get; init; } = 0m;

    /// <summary>
    /// Periodic rate (e.g., 0.10 for 10% per period). When set, takes precedence over <see cref="AnnualRate"/>.
    /// </summary>
    public decimal PeriodRate { get; init; } = 0m;

    /// <summary>
    /// Increment added to the periodic rate for each new period.
    /// </summary>
    public decimal PeriodRateIncrement { get; init; } = 0m;

    /// <summary>
    /// Optional cap for penalty amount.
    /// </summary>
    public decimal? Cap { get; init; }

    /// <summary>
    /// Optional minimum for penalty amount.
    /// </summary>
    public decimal? Minimum { get; init; }

    /// <summary>
    /// Whether penalties are compounded on base.
    /// </summary>
    public bool Capitalize { get; init; } = false;

    /// <summary>
    /// Validate definition values.
    /// </summary>
    public void Validate()
    {
        if (FixedAmount < 0) throw new ArgumentOutOfRangeException(nameof(FixedAmount));
        if (GraceDays < 0) throw new ArgumentOutOfRangeException(nameof(GraceDays));
        if (PeriodDays <= 0) throw new ArgumentOutOfRangeException(nameof(PeriodDays));
        if (AnnualRate < 0) throw new ArgumentOutOfRangeException(nameof(AnnualRate));
        if (PeriodRate < 0) throw new ArgumentOutOfRangeException(nameof(PeriodRate));
        if (PeriodRateIncrement < 0) throw new ArgumentOutOfRangeException(nameof(PeriodRateIncrement));
    }
}
