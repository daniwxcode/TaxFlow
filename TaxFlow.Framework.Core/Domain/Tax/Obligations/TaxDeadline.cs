using Core.Domain.Contracts.Abstracts;
using Core.Domain.Tax.Penalties;

namespace Core.Domain.Tax.Obligations;

/// <summary>
/// Type of tax obligation deadline.
/// </summary>
public enum DeadlineType
{
    /// <summary>
    /// Declaration/filing deadline.
    /// </summary>
    Declaration = 1,

    /// <summary>
    /// Payment deadline.
    /// </summary>
    Payment = 2
}

/// <summary>
/// Represents a deadline for a tax obligation (declaration or payment).
/// </summary>
public abstract class TaxDeadline : AuditableEntity
{
    /// <summary>
    /// Unique key identifying this deadline within a tax rule.
    /// </summary>
    public string Key { get; init; } = string.Empty;

    /// <summary>
    /// Human-readable label for the deadline.
    /// </summary>
    public string Label { get; init; } = string.Empty;

    /// <summary>
    /// Type of deadline.
    /// </summary>
    public abstract DeadlineType Type { get; }

    /// <summary>
    /// Due date for this deadline.
    /// </summary>
    public DateTimeOffset DueDate { get; init; }

    /// <summary>
    /// Grace period after the due date before penalties apply.
    /// Supports days, weeks, months, or years.
    /// </summary>
    public Duration GracePeriod { get; init; } = Duration.Zero;

    /// <summary>
    /// Gets the effective due date including grace period.
    /// </summary>
    public DateTimeOffset EffectiveDueDate => GracePeriod.AddTo(DueDate);

    /// <summary>
    /// Optional description for this deadline.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Whether this deadline is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Checks if the deadline is overdue as of the given date.
    /// </summary>
    public bool IsOverdue(DateTimeOffset asOf) => asOf > EffectiveDueDate;

    /// <summary>
    /// Calculates the number of days late as of the given date.
    /// </summary>
    public int GetDaysLate(DateTimeOffset asOf)
    {
        if (!IsOverdue(asOf))
            return 0;

        return (int)(asOf.Date - EffectiveDueDate.Date).TotalDays;
    }

    /// <summary>
    /// Calculates the time elapsed since the effective due date.
    /// </summary>
    public TimeSpan GetTimeOverdue(DateTimeOffset asOf)
    {
        if (!IsOverdue(asOf))
            return TimeSpan.Zero;

        return asOf - EffectiveDueDate;
    }
}
