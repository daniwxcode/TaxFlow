using Core.Domain.Contracts;
using Core.Domain.Contracts.Abstracts;
using Core.Domain.Tax.Obligations;
using Core.Domain.Tax.Penalties;

using System;

namespace Core.Domain.Tax.Calculation;

/// <summary>
/// Represents a tax rule attached to an asset type. The rule contains a dynamic expression
/// (NCalc) used to compute the taxable amount or tax due based on asset attributes.
/// </summary>
public class TaxRule : TemporalAuditableEntity
{
    /// <summary>
    /// Unique key for the rule.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Friendly label for the rule.
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// The NCalc expression that computes the tax. It receives variables based on asset attributes.
    /// </summary>
    public string Expression { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the rule.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Whether the rule is enabled and should be executed.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Schedule of obligations (declaration and payment deadlines) for this tax rule.
    /// </summary>
    public TaxObligationSchedule? ObligationSchedule { get; private set; }

    /// <summary>
    /// Configures the obligation schedule for this tax rule.
    /// </summary>
    /// <param name="schedule">Obligation schedule to assign.</param>
    public TaxRule ConfigureObligationSchedule(TaxObligationSchedule schedule)
    {
        ArgumentNullException.ThrowIfNull(schedule);

        var validation = schedule.Validate();
        if (validation.HasErrors)
            throw new ArgumentException($"Invalid obligation schedule: {validation.ErrorMessage}");

        ObligationSchedule = schedule;
        return this;
    }

    /// <summary>
    /// Creates an obligation schedule for this rule if one doesn't exist.
    /// </summary>
    public TaxObligationSchedule GetOrCreateObligationSchedule()
    {
        ObligationSchedule ??= TaxObligationSchedule.Create();
        return ObligationSchedule;
    }

    /// <summary>
    /// Checks if this rule has any declaration deadlines configured.
    /// </summary>
    public bool HasDeclarationDeadline => ObligationSchedule?.HasDeclarationDeadline == true;

    /// <summary>
    /// Checks if this rule has any payment deadlines configured.
    /// </summary>
    public bool HasPaymentDeadlines => ObligationSchedule?.HasPaymentDeadlines == true;

    /// <summary>
    /// Gets all overdue deadlines for this rule as of the given date.
    /// </summary>
    public IReadOnlyList<TaxDeadline> GetOverdueDeadlines(DateTimeOffset asOf)
    {
        return ObligationSchedule?.GetOverdueDeadlines(asOf) ?? [];
    }
}
