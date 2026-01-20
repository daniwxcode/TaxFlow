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
    /// Type of payment (full, advance, installment, balance, etc.).
    /// </summary>
    public PaymentType PaymentType { get; init; } = PaymentType.Full;

    /// <summary>
    /// Penalty definition applied when this payment deadline is missed.
    /// This typically represents "pénalité de recouvrement" for late payment.
    /// </summary>
    public PenaltyDefinition? PenaltyDefinition { get; private set; }

    /// <summary>
    /// Reference to the declaration deadline this payment is linked to (if any).
    /// </summary>
    public string? LinkedDeclarationKey { get; init; }

    /// <summary>
    /// Whether this payment can be split into sub-payments.
    /// </summary>
    public bool AllowsPartialPayment { get; init; } = true;

    /// <summary>
    /// Minimum payment amount (if partial payment is allowed).
    /// </summary>
    public decimal? MinimumPayment { get; init; }

    /// <summary>
    /// Fixed amount due (if applicable, overrides fraction calculation).
    /// </summary>
    public decimal? FixedAmount { get; init; }

    /// <summary>
    /// Creates a new payment deadline.
    /// </summary>
    /// <param name="key">Unique key for this deadline.</param>
    /// <param name="label">Human-readable label.</param>
    /// <param name="dueDate">Due date for the payment.</param>
    /// <param name="fraction">Fraction of total amount due (0.0 to 1.0).</param>
    /// <param name="order">Order in the payment schedule.</param>
    /// <param name="gracePeriod">Grace period before penalties apply.</param>
    public static PaymentDeadline Create(
        string key,
        string label,
        DateTimeOffset dueDate,
        decimal fraction = 1.0m,
        int order = 1,
        Duration gracePeriod = default)
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
            GracePeriod = gracePeriod
        };
    }

    /// <summary>
    /// Creates a new payment deadline with full configuration.
    /// </summary>
    public static PaymentDeadline Create(
        string key,
        string label,
        DateTimeOffset dueDate,
        PaymentType paymentType,
        decimal fraction = 1.0m,
        int order = 1,
        DeadlinePeriodicity periodicity = DeadlinePeriodicity.OneTime,
        TaxRegime regime = TaxRegime.General,
        Duration gracePeriod = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(label);

        if (fraction <= 0 || fraction > 1)
            throw new ArgumentOutOfRangeException(nameof(fraction), "Fraction must be between 0 (exclusive) and 1 (inclusive).");

        return new PaymentDeadline
        {
            Key = key.Trim(),
            Label = label.Trim(),
            DueDate = dueDate,
            Fraction = fraction,
            Order = order,
            GracePeriod = gracePeriod,
            PaymentType = paymentType,
            Periodicity = periodicity,
            Regime = regime
        };
    }

    /// <summary>
    /// Creates an advance payment deadline (acompte).
    /// </summary>
    public static PaymentDeadline CreateAdvance(
        string key,
        string label,
        DateTimeOffset dueDate,
        decimal fraction,
        int order,
        Duration gracePeriod = default)
    {
        return new PaymentDeadline
        {
            Key = key.Trim(),
            Label = label.Trim(),
            DueDate = dueDate,
            Fraction = fraction,
            Order = order,
            GracePeriod = gracePeriod,
            PaymentType = PaymentType.Advance
        };
    }

    /// <summary>
    /// Creates a balance payment deadline (solde).
    /// </summary>
    public static PaymentDeadline CreateBalance(
        string key,
        string label,
        DateTimeOffset dueDate,
        decimal fraction,
        int order,
        Duration gracePeriod = default)
    {
        return new PaymentDeadline
        {
            Key = key.Trim(),
            Label = label.Trim(),
            DueDate = dueDate,
            Fraction = fraction,
            Order = order,
            GracePeriod = gracePeriod,
            PaymentType = PaymentType.Balance
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
    /// Links this payment to a declaration deadline.
    /// </summary>
    public PaymentDeadline LinkedToDeclaration(string declarationKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(declarationKey);
        return new PaymentDeadline
        {
            Key = Key,
            Label = Label,
            DueDate = DueDate,
            GracePeriod = GracePeriod,
            Fraction = Fraction,
            Order = Order,
            PaymentType = PaymentType,
            Periodicity = Periodicity,
            Regime = Regime,
            Description = Description,
            Enabled = Enabled,
            LinkedDeclarationKey = declarationKey.Trim(),
            AllowsPartialPayment = AllowsPartialPayment,
            MinimumPayment = MinimumPayment,
            FixedAmount = FixedAmount,
            ConditionExpression = ConditionExpression,
            FiscalYear = FiscalYear,
            Period = Period,
            PenaltyDefinition = PenaltyDefinition
        };
    }

    /// <summary>
    /// Adds a legal reference to this payment deadline.
    /// </summary>
    public new PaymentDeadline AddLegalReference(LegalReference reference)
    {
        base.AddLegalReference(reference);
        return this;
    }

    /// <summary>
    /// Calculates the amount due at this deadline based on the total tax amount.
    /// </summary>
    /// <param name="totalTaxAmount">Total tax amount.</param>
    public decimal GetAmountDue(decimal totalTaxAmount)
    {
        if (FixedAmount.HasValue)
            return FixedAmount.Value;

        if (totalTaxAmount < 0)
            throw new ArgumentOutOfRangeException(nameof(totalTaxAmount));

        return totalTaxAmount * Fraction;
    }

    /// <summary>
    /// Checks if a penalty applies for this deadline as of the given date.
    /// </summary>
    public bool HasPenalty(DateTimeOffset asOf) => PenaltyDefinition is not null && IsOverdue(asOf);

    /// <summary>
    /// Whether this is an advance payment.
    /// </summary>
    public bool IsAdvancePayment => PaymentType == PaymentType.Advance;

    /// <summary>
    /// Whether this is a balance/final payment.
    /// </summary>
    public bool IsBalancePayment => PaymentType == PaymentType.Balance;
}
