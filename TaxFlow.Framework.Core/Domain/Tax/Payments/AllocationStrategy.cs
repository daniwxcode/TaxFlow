namespace Core.Domain.Tax.Payments;

/// <summary>
/// Payment allocation strategy for installments.
/// </summary>
public enum AllocationStrategy
{
    /// <summary>
    /// Allocate payments to earliest due installments first.
    /// </summary>
    FifoByDueDate = 1
}
