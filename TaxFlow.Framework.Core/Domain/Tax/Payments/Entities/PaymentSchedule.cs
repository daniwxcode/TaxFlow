using System.Collections.ObjectModel;

namespace Core.Domain.Tax.Payments;

/// <summary>
/// Represents a payment schedule with installments and payment allocations.
/// </summary>
public sealed class PaymentSchedule
{
    private readonly List<Installment> _installments = new();

    /// <summary>
    /// Create a schedule from a list of installments.
    /// </summary>
    /// <param name="installments">Installments in the schedule.</param>
    public PaymentSchedule(IEnumerable<Installment> installments)
    {
        ArgumentNullException.ThrowIfNull(installments);
        _installments.AddRange(installments);
        DeclarationId = Guid.Empty;
        LiquidationId = null;
    }

    /// <summary>
    /// Create a schedule for a declaration/liquidation.
    /// </summary>
    /// <param name="declarationId">Declaration identifier.</param>
    /// <param name="liquidationId">Liquidation identifier (optional).</param>
    /// <param name="installments">Installments in the schedule.</param>
    public PaymentSchedule(Guid declarationId, Guid? liquidationId, IEnumerable<Installment> installments)
    {
        ArgumentNullException.ThrowIfNull(installments);
        DeclarationId = declarationId == Guid.Empty ? Guid.NewGuid() : declarationId;
        LiquidationId = liquidationId;
        _installments.AddRange(installments);
    }

    /// <summary>
    /// Declaration identifier.
    /// </summary>
    public Guid DeclarationId { get; }

    /// <summary>
    /// Liquidation identifier (optional).
    /// </summary>
    public Guid? LiquidationId { get; }

    /// <summary>
    /// Installments in the schedule.
    /// </summary>
    public IReadOnlyCollection<Installment> Installments => new ReadOnlyCollection<Installment>(_installments);

    /// <summary>
    /// Total amount due for the schedule.
    /// </summary>
    public decimal TotalDue => _installments.Sum(i => i.AmountDue);

    /// <summary>
    /// Apply a payment to installments using the provided allocation strategy (default FIFO by due date).
    /// </summary>
    /// <param name="payment">Payment to apply.</param>
    /// <param name="strategy">Allocation strategy.</param>
    public void ApplyPayment(Payment payment, AllocationStrategy strategy = AllocationStrategy.FifoByDueDate)
    {
        ArgumentNullException.ThrowIfNull(payment);
        if (_installments.Count == 0)
        {
            return;
        }

        decimal remaining = payment.Amount;
        List<Installment> ordered = strategy switch
        {
            AllocationStrategy.FifoByDueDate => _installments.OrderBy(i => i.DueDate).ToList(),
            _ => _installments.OrderBy(i => i.DueDate).ToList()
        };

        for (var i = 0; i < ordered.Count && remaining > 0; i++)
        {
            Installment inst = ordered[i];
            decimal outstanding = inst.GetOutstanding();
            if (outstanding <= 0)
            {
                continue;
            }

            decimal allocated = remaining > outstanding ? outstanding : remaining;
            inst.ApplyAllocation(new PaymentAllocation(payment.Id, inst.Id, allocated, payment.PaidOn));
            remaining -= allocated;
        }
    }
}
