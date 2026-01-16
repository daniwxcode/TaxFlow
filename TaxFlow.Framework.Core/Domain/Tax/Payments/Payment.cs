using System;

namespace Core.Domain.Tax.Payments;

/// <summary>
/// Represents a payment made by the taxpayer.
/// </summary>
public sealed class Payment
{
    public Payment(Guid id, decimal amount, DateTimeOffset paidOn)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Amount = amount;
        PaidOn = paidOn;
    }

    public Guid Id { get; }
    public decimal Amount { get; }
    public DateTimeOffset PaidOn { get; }
}
