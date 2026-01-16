using Core.Domain.Tax.Penalties;

namespace Core.Domain.Tax.Obligations;

/// <summary>
/// Represents a payment deadline with the fraction of the total amount due
/// and associated penalty rules for late payment.
/// </summary>
public sealed class PaymentDeadline : TaxDeadline
{
    /// <inheritdoc />
    public override DeadlineType Type => DeadlineType.Payment;

    /// <summary>
    /// The fraction of the total tax amount due at this deadline (0.0 to 1.0).
    /// For example, 0.5 means 50% of the total is due.
    /// </summary>
    public decimal Fraction { get; init; } = 1.0m;

    /// <summary>
    /// Order of this payment deadline within a schedule (1-based).
    /// Used for installment numbering.
    /// </summary>
    public int Order { get; init; } = 1;

    /// <summary>
    /// Penalty definition applied when this payment deadline is missed.
    /// This typically represents "pénalité de recouvrement" for late payment.
    /// </summary>
    public PenaltyDefinition? PenaltyDefinition { get; private set; }

    /// <summary>
    /// Creates a new payment deadline.
    /// </summary>
    /// <param name="key">Unique key for this deadline.</param>
    /// <param name="label">Human-readable label.</param>
    /// <param name="dueDate">Due date for the payment.</param>
    /// <param name="fraction">Fraction of total amount due (0.0 to 1.0).</param>
    /// <param name="order">Order in the payment schedule.</param>
    /// <param name="graceDays">Grace period in days.</param>
    public static PaymentDeadline Create(
        string key,
        string label,
        DateTimeOffset dueDate,
        decimal fraction = 1.0m,
        int order = 1,
        int graceDays = 0)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(label);

        if (fraction <= 0 || fraction > 1)
            throw new ArgumentOutOfRangeException(nameof(fraction), "Fraction must be between 0 (exclusive) and 1 (inclusive).");

        if (order < 1)
            throw new ArgumentOutOfRangeException(nameof(order), "Order must be at least 1.");

        return new PaymentDeadline
        {
            Key = key.Trim(),
            Label = label.Trim(),
            DueDate = dueDate,
            Fraction = fraction,
            Order = order,
            GraceDays = graceDays
        };
    }

    /// <summary>
    /// Configures the penalty definition for late payment.
    /// </summary>
    /// <param name="definition">Penalty definition to apply.</param>
    public PaymentDeadline WithPenalty(PenaltyDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        PenaltyDefinition = definition;
        return this;
    }

    /// <summary>
    /// Calculates the amount due at this deadline based on the total tax amount.
    /// </summary>
    /// <param name="totalTaxAmount">Total tax amount.</param>
    public decimal GetAmountDue(decimal totalTaxAmount)
    {
        if (totalTaxAmount < 0)
            throw new ArgumentOutOfRangeException(nameof(totalTaxAmount));

        return totalTaxAmount * Fraction;
    }

    /// <summary>
    /// Checks if a penalty applies for this deadline as of the given date.
    /// </summary>
    public bool HasPenalty(DateTimeOffset asOf) => PenaltyDefinition is not null && IsOverdue(asOf);
}
