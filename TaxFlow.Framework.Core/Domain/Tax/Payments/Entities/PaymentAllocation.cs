using System;

namespace Core.Domain.Tax.Payments;

/// <summary>
/// Represents an allocation of a payment to a specific installment.
/// </summary>
public sealed class PaymentAllocation
{
    public PaymentAllocation(Guid paymentId, Guid installmentId, decimal amount, DateTimeOffset appliedOn)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
        PaymentId = paymentId;
        InstallmentId = installmentId;
        Amount = amount;
        AppliedOn = appliedOn;
    }

    public Guid PaymentId { get; }
    public Guid InstallmentId { get; }
    public decimal Amount { get; }
    public DateTimeOffset AppliedOn { get; }
}
