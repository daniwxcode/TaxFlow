namespace Core.Domain.Tax.Payments;

/// <summary>
/// Represents an allocation of a payment to a specific installment.
/// </summary>
public sealed class PaymentAllocation
{
    /// <summary>
    /// constructs a new payment allocation.
    /// </summary>
    /// <param name="paymentId"></param>
    /// <param name="installmentId"></param>
    /// <param name="amount"></param>
    /// <param name="appliedOn"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public PaymentAllocation(Guid paymentId, Guid installmentId, decimal amount, DateTimeOffset appliedOn)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
        PaymentId = paymentId;
        InstallmentId = installmentId;
        Amount = amount;
        AppliedOn = appliedOn;
    }
    /// <summary>
    /// Identifier of the payment.
    /// </summary>
    public Guid PaymentId { get; }
    /// <summary>
    /// Installment identifier.
    /// </summary>
    public Guid InstallmentId { get; }
    /// <summary>
    /// Amount allocated to the installment.
    /// </summary>
    public decimal Amount { get; }
    /// <summary>
    /// Date when the allocation was applied.
    /// </summary>
    public DateTimeOffset AppliedOn { get; }
}
