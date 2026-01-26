using System;

namespace Core.Domain.Tax.Payments;

/// <summary>
/// Represents a payment made by the taxpayer.
/// </summary>
public sealed class Payment
{
    /// <summary>
    /// Create a new payment.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="amount"></param>
    /// <param name="paidOn"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Payment(Guid id, decimal amount, DateTimeOffset paidOn)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Amount = amount;
        PaidOn = paidOn;
    }
    /// <summary>
    /// identifier of the payment.
    /// </summary>
    public Guid Id { get; }
    /// <summary>
    /// Amount paid.
    /// </summary>
    public decimal Amount { get; }
    /// <summary>
    /// Occurrence date of the payment.
    /// </summary>
    public DateTimeOffset PaidOn { get; }
}
