namespace Core.Domain.Tax.Penalties;

/// <summary>
/// Helper methods shared across penalty rules.
/// Eliminates code duplication in penalty calculation logic.
/// </summary>
internal static class PenaltyCalculationHelper
{
    /// <summary>
    /// Calculates prorated amount based on annual rate and days.
    /// </summary>
    /// <param name="baseAmount">Base amount for calculation.</param>
    /// <param name="annualRate">Annual rate to apply.</param>
    /// <param name="daysInYear">Days in year for proration.</param>
    /// <param name="days">Number of days to prorate.</param>
    /// <returns>Prorated amount.</returns>
    public static decimal Prorate(decimal baseAmount, decimal annualRate, int daysInYear, int days)
    {
        if (baseAmount <= 0 || annualRate <= 0 || days <= 0)
            return 0m;

        return baseAmount * annualRate * days / daysInYear;
    }

    /// <summary>
    /// Applies minimum floor and maximum cap to a value.
    /// </summary>
    /// <param name="value">Value to constrain.</param>
    /// <param name="minimum">Optional minimum value.</param>
    /// <param name="cap">Optional maximum value.</param>
    /// <returns>Constrained value.</returns>
    public static decimal ApplyFloorAndCap(decimal value, decimal? minimum, decimal? cap)
    {
        if (minimum.HasValue && value < minimum.Value)
            value = minimum.Value;

        if (cap.HasValue && value > cap.Value)
            value = cap.Value;

        return value;
    }

    /// <summary>
    /// Calculates the number of late days from an effective due date.
    /// </summary>
    /// <param name="asOf">Calculation date.</param>
    /// <param name="effectiveDueDate">Effective due date.</param>
    /// <returns>Number of days late (0 if not late).</returns>
    public static int CalculateDaysLate(DateTimeOffset asOf, DateTimeOffset effectiveDueDate)
    {
        if (asOf <= effectiveDueDate)
            return 0;

        return (int)Math.Max(0, (asOf.Date - effectiveDueDate.Date).TotalDays);
    }

    /// <summary>
    /// Calculates the number of periods based on days late and period length.
    /// </summary>
    /// <param name="daysLate">Number of days late.</param>
    /// <param name="periodDays">Days per period.</param>
    /// <returns>Number of periods.</returns>
    public static int CalculatePeriodCount(int daysLate, int periodDays)
    {
        if (daysLate <= 0 || periodDays <= 0)
            return 0;

        return (int)Math.Ceiling(daysLate / (double)periodDays);
    }

    /// <summary>
    /// Gets the declaration ID, generating one if empty.
    /// </summary>
    public static Guid GetOrCreateDeclarationId(Guid declarationId)
    {
        return declarationId == Guid.Empty ? Guid.NewGuid() : declarationId;
    }

    /// <summary>
    /// Checks if a trigger event matches the definition's trigger.
    /// </summary>
    public static bool MatchesTriggerEvent(PenaltyTriggerEvent definitionEvent, PenaltyTriggerEvent requestedEvent)
    {
        return definitionEvent == PenaltyTriggerEvent.Any || definitionEvent.HasFlag(requestedEvent);
    }
}
