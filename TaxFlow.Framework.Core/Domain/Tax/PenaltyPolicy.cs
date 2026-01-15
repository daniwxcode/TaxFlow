using System;

namespace Core.Domain.Tax;

/// <summary>
/// Defines penalty calculation rules for assiette and recouvrement.
/// </summary>
public sealed class PenaltyPolicy
{
    /// <summary>
    /// Fixed assiette penalty amount (applied once per declaration).
    /// </summary>
    public decimal AssietteFixedAmount { get; init; } = 0m;

    /// <summary>
    /// Grace period in days before assiette penalties start.
    /// </summary>
    public int AssietteGraceDays { get; init; } = 0;

    /// <summary>
    /// Periodicity in days for assiette penalties (default 30).
    /// </summary>
    public int AssiettePeriodDays { get; init; } = 30;

    /// <summary>
    /// Annual rate for assiette penalties.
    /// </summary>
    public decimal AssietteAnnualRate { get; init; } = 0m;

    /// <summary>
    /// Annual rate for collection penalties.
    /// </summary>
    public decimal RecouvrementAnnualRate { get; init; } = 0m;

    /// <summary>
    /// Periodic rate for collection penalties (e.g., 0.10 for 10% per period).
    /// When set, takes precedence over <see cref="RecouvrementAnnualRate"/>.
    /// </summary>
    public decimal RecouvrementPeriodRate { get; init; } = 0m;

    /// <summary>
    /// Increment added to the periodic rate for each new period (e.g., 0.01 for +1% per period).
    /// </summary>
    public decimal RecouvrementPeriodRateIncrement { get; init; } = 0m;

    /// <summary>
    /// Number of days in the base period for proration (default 365).
    /// </summary>
    public int DaysInYear { get; init; } = 365;

    /// <summary>
    /// Grace period in days before collection penalties start.
    /// </summary>
    public int RecouvrementGraceDays { get; init; } = 0;

    /// <summary>
    /// Periodicity in days for recouvrement penalties (default 30).
    /// </summary>
    public int RecouvrementPeriodDays { get; init; } = 30;

    /// <summary>
    /// Optional cap for assiette penalty amount.
    /// </summary>
    public decimal? AssietteCap { get; init; }

    /// <summary>
    /// Optional cap for recouvrement penalty amount.
    /// </summary>
    public decimal? RecouvrementCap { get; init; }

    /// <summary>
    /// Optional minimum for assiette penalty amount.
    /// </summary>
    public decimal? AssietteMinimum { get; init; }

    /// <summary>
    /// Optional minimum for recouvrement penalty amount.
    /// </summary>
    public decimal? RecouvrementMinimum { get; init; }

    /// <summary>
    /// Minimum acceptable amount for a penalty line (lines below are skipped).
    /// </summary>
    public decimal MinimumLineAmount { get; init; } = 0m;

    /// <summary>
    /// Whether recouvrement penalties are compounded on unpaid balance.
    /// </summary>
    public bool CapitalizeRecouvrement { get; init; } = false;

    /// <summary>
    /// Whether assiette penalties are compounded on tax base.
    /// </summary>
    public bool CapitalizeAssiette { get; init; } = false;

    /// <summary>
    /// Validate policy values.
    /// </summary>
    public void Validate()
    {
        if (DaysInYear <= 0) throw new ArgumentOutOfRangeException(nameof(DaysInYear));
        if (AssietteAnnualRate < 0) throw new ArgumentOutOfRangeException(nameof(AssietteAnnualRate));
        if (AssietteFixedAmount < 0) throw new ArgumentOutOfRangeException(nameof(AssietteFixedAmount));
        if (AssietteGraceDays < 0) throw new ArgumentOutOfRangeException(nameof(AssietteGraceDays));
        if (AssiettePeriodDays <= 0) throw new ArgumentOutOfRangeException(nameof(AssiettePeriodDays));
        if (RecouvrementAnnualRate < 0) throw new ArgumentOutOfRangeException(nameof(RecouvrementAnnualRate));
        if (RecouvrementPeriodRate < 0) throw new ArgumentOutOfRangeException(nameof(RecouvrementPeriodRate));
        if (RecouvrementPeriodRateIncrement < 0) throw new ArgumentOutOfRangeException(nameof(RecouvrementPeriodRateIncrement));
        if (RecouvrementGraceDays < 0) throw new ArgumentOutOfRangeException(nameof(RecouvrementGraceDays));
        if (RecouvrementPeriodDays <= 0) throw new ArgumentOutOfRangeException(nameof(RecouvrementPeriodDays));
        if (MinimumLineAmount < 0) throw new ArgumentOutOfRangeException(nameof(MinimumLineAmount));
    }
}
