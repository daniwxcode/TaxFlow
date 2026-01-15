namespace Core.Domain.Tax;

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
