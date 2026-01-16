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
    /// Grace period before penalties start.
    /// Supports days, weeks, months, or years.
    /// </summary>
    public Duration GracePeriod { get; init; } = Duration.Zero;

    /// <summary>
    /// Grace period in days (for backward compatibility).
    /// </summary>
    [Obsolete("Use GracePeriod instead for more flexibility.")]
    public int GraceDays
    {
        get => GracePeriod.ToDays();
        init => GracePeriod = Duration.Days(value);
    }

    /// <summary>
    /// Periodicity for penalties.
    /// Supports days, weeks, months, or years.
    /// </summary>
    public Duration Period { get; init; } = Duration.Days(30);

    /// <summary>
    /// Periodicity in days for penalties (for backward compatibility).
    /// </summary>
    [Obsolete("Use Period instead for more flexibility.")]
    public int PeriodDays
    {
        get => Period.ToDays();
        init => Period = Duration.Days(value);
    }

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
        if (GracePeriod.Value < 0) throw new ArgumentOutOfRangeException(nameof(GracePeriod));
        if (Period.Value <= 0) throw new ArgumentOutOfRangeException(nameof(Period));
        if (AnnualRate < 0) throw new ArgumentOutOfRangeException(nameof(AnnualRate));
        if (PeriodRate < 0) throw new ArgumentOutOfRangeException(nameof(PeriodRate));
        if (PeriodRateIncrement < 0) throw new ArgumentOutOfRangeException(nameof(PeriodRateIncrement));
    }

    /// <summary>
    /// Calculates the effective due date by adding the grace period to the original due date.
    /// </summary>
    public DateTimeOffset GetEffectiveDueDate(DateTimeOffset dueDate) => GracePeriod.AddTo(dueDate);

    /// <summary>
    /// Gets the start date of a specific period.
    /// </summary>
    public DateTimeOffset GetPeriodStartDate(DateTimeOffset effectiveDueDate, int periodIndex)
    {
        if (periodIndex < 1)
            throw new ArgumentOutOfRangeException(nameof(periodIndex), "Period index must be at least 1.");

        var start = effectiveDueDate;
        for (var i = 1; i < periodIndex; i++)
        {
            start = Period.AddTo(start);
        }
        return start;
    }

    /// <summary>
    /// Gets the end date of a specific period.
    /// </summary>
    public DateTimeOffset GetPeriodEndDate(DateTimeOffset effectiveDueDate, int periodIndex)
    {
        return Period.AddTo(GetPeriodStartDate(effectiveDueDate, periodIndex));
    }
}
