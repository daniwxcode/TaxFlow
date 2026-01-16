using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Core.Domain.Tax.Payments;

/// <summary>
/// Represents a scheduled installment for tax payment.
/// </summary>
public sealed class Installment
{
    private readonly List<PaymentAllocation> _allocations = new();

    /// <summary>
    /// Create a new installment.
    /// </summary>
    /// <param name="id">Installment identifier (auto-generated if empty).</param>
    /// <param name="amountDue">Amount due for the installment.</param>
    /// <param name="dueDate">Due date of the installment.</param>
    /// <param name="graceDays">Grace period in days.</param>
    public Installment(Guid id, decimal amountDue, DateTimeOffset dueDate, int graceDays = 0)
    {
        if (amountDue < 0) throw new ArgumentOutOfRangeException(nameof(amountDue));
        if (graceDays < 0) throw new ArgumentOutOfRangeException(nameof(graceDays));

        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        AmountDue = amountDue;
        DueDate = dueDate;
        GraceDays = graceDays;
    }

    /// <summary>
    /// Installment identifier.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Amount due for this installment.
    /// </summary>
    public decimal AmountDue { get; }

    /// <summary>
    /// Due date for this installment.
    /// </summary>
    public DateTimeOffset DueDate { get; }

    /// <summary>
    /// Grace period in days.
    /// </summary>
    public int GraceDays { get; }

    /// <summary>
    /// Payment allocations applied to this installment.
    /// </summary>
    public IReadOnlyCollection<PaymentAllocation> Allocations => new ReadOnlyCollection<PaymentAllocation>(_allocations);

    /// <summary>
    /// Effective due date including grace period.
    /// </summary>
    public DateTimeOffset EffectiveDueDate => DueDate.AddDays(GraceDays);

    /// <summary>
    /// Apply a payment allocation.
    /// </summary>
    /// <param name="allocation">Allocation to apply.</param>
    public void ApplyAllocation(PaymentAllocation allocation)
    {
        if (allocation is null) throw new ArgumentNullException(nameof(allocation));
        if (allocation.Amount <= 0) return;
        _allocations.Add(allocation);
    }

    /// <summary>
    /// Total paid amount, optionally up to a given date.
    /// </summary>
    /// <param name="upTo">Upper bound date for payments.</param>
    public decimal GetPaidAmount(DateTimeOffset? upTo = null)
    {
        var total = 0m;
        for (var i = 0; i < _allocations.Count; i++)
        {
            var a = _allocations[i];
            if (upTo == null || a.AppliedOn <= upTo)
                total += a.Amount;
        }
        return total;
    }

    /// <summary>
    /// Total paid on time, optionally up to a given date.
    /// </summary>
    /// <param name="upTo">Upper bound date for payments.</param>
    public decimal GetPaidOnTime(DateTimeOffset? upTo = null)
    {
        var total = 0m;
        var deadline = EffectiveDueDate;
        for (var i = 0; i < _allocations.Count; i++)
        {
            var a = _allocations[i];
            if (a.AppliedOn <= deadline && (upTo == null || a.AppliedOn <= upTo))
                total += a.Amount;
        }
        return total;
    }

    /// <summary>
    /// Outstanding amount (never negative).
    /// </summary>
    /// <param name="upTo">Upper bound date for payments.</param>
    public decimal GetOutstanding(DateTimeOffset? upTo = null)
    {
        var paid = GetPaidAmount(upTo);
        var outstanding = AmountDue - paid;
        return outstanding < 0 ? 0 : outstanding;
    }
}
