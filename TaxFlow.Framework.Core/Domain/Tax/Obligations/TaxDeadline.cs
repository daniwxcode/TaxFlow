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
    /// For recurring deadlines, this represents the reference date or first occurrence.
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
    /// Periodicity of the deadline (monthly, quarterly, annual, etc.).
    /// </summary>
    public DeadlinePeriodicity Periodicity { get; init; } = DeadlinePeriodicity.Annual;

    /// <summary>
    /// Tax regime this deadline applies to.
    /// </summary>
    public TaxRegime Regime { get; init; } = TaxRegime.General;

    /// <summary>
    /// Order of this deadline within its type (for sorting multiple deadlines).
    /// </summary>
    public int Order { get; init; } = 1;

    /// <summary>
    /// Legal reference(s) that justify this obligation.
    /// </summary>
    public IReadOnlyList<LegalReference> LegalReferences => _legalReferences.AsReadOnly();
    private readonly List<LegalReference> _legalReferences = [];

    /// <summary>
    /// Optional condition expression that determines if this deadline applies.
    /// Uses the same NCalc syntax as tax rules.
    /// </summary>
    public string? ConditionExpression { get; init; }

    /// <summary>
    /// Fiscal year this deadline belongs to (if applicable).
    /// </summary>
    public int? FiscalYear { get; init; }

    /// <summary>
    /// Period within the fiscal year (1-12 for months, 1-4 for quarters, etc.).
    /// </summary>
    public int? Period { get; init; }

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

    /// <summary>
    /// Adds a legal reference to this deadline.
    /// </summary>
    public TaxDeadline AddLegalReference(LegalReference reference)
    {
        ArgumentNullException.ThrowIfNull(reference);
        _legalReferences.Add(reference);
        return this;
    }

    /// <summary>
    /// Removes all legal references.
    /// </summary>
    public TaxDeadline ClearLegalReferences()
    {
        _legalReferences.Clear();
        return this;
    }

    /// <summary>
    /// Checks if this deadline has at least one legal reference.
    /// </summary>
    public bool HasLegalBasis => _legalReferences.Count > 0;

    /// <summary>
    /// Gets a formatted string of all legal references.
    /// </summary>
    public string GetLegalBasisSummary()
    {
        if (_legalReferences.Count == 0)
            return string.Empty;

        return string.Join("; ", _legalReferences.Select(r => r.GetCitation()));
    }

    /// <summary>
    /// Calculates the next occurrence of this deadline based on periodicity.
    /// </summary>
    /// <param name="after">Calculate next occurrence after this date.</param>
    /// <returns>Next occurrence date, or null if not recurring.</returns>
    public DateTimeOffset? GetNextOccurrence(DateTimeOffset after)
    {
        if (Periodicity == DeadlinePeriodicity.OneTime || Periodicity == DeadlinePeriodicity.EventDriven)
            return null;

        var current = DueDate;
        while (current <= after)
        {
            current = Periodicity switch
            {
                DeadlinePeriodicity.Monthly => current.AddMonths(1),
                DeadlinePeriodicity.Quarterly => current.AddMonths(3),
                DeadlinePeriodicity.SemiAnnual => current.AddMonths(6),
                DeadlinePeriodicity.Annual => current.AddYears(1),
                _ => current.AddYears(1)
            };
        }

        return current;
    }

    /// <summary>
    /// Gets all occurrences of this deadline within a date range.
    /// </summary>
    public IEnumerable<DateTimeOffset> GetOccurrences(DateTimeOffset from, DateTimeOffset to)
    {
        if (Periodicity == DeadlinePeriodicity.OneTime)
        {
            if (DueDate >= from && DueDate <= to)
                yield return DueDate;
            yield break;
        }

        if (Periodicity == DeadlinePeriodicity.EventDriven)
            yield break;

        var current = DueDate;
        
        // Find the first occurrence >= from
        while (current < from)
        {
            current = Periodicity switch
            {
                DeadlinePeriodicity.Monthly => current.AddMonths(1),
                DeadlinePeriodicity.Quarterly => current.AddMonths(3),
                DeadlinePeriodicity.SemiAnnual => current.AddMonths(6),
                DeadlinePeriodicity.Annual => current.AddYears(1),
                _ => current.AddYears(1)
            };
        }

        // Yield all occurrences <= to
        while (current <= to)
        {
            yield return current;
            current = Periodicity switch
            {
                DeadlinePeriodicity.Monthly => current.AddMonths(1),
                DeadlinePeriodicity.Quarterly => current.AddMonths(3),
                DeadlinePeriodicity.SemiAnnual => current.AddMonths(6),
                DeadlinePeriodicity.Annual => current.AddYears(1),
                _ => current.AddYears(1)
            };
        }
    }
}
