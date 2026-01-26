using System.Collections.ObjectModel;

namespace Core.Domain.Tax.Penalties;

/// <summary>
/// Result of penalty calculation with totals and details.
/// </summary>
public sealed class PenaltyCalculationResult
{
    /// <summary>
    /// Create a penalty calculation result.
    /// </summary>
    /// <param name="accruals">Accrual entries.</param>
    public PenaltyCalculationResult(IEnumerable<PenaltyAccrual> accruals)
    {
        var list = (accruals ?? new List<PenaltyAccrual>()).ToList();
        Accruals = new ReadOnlyCollection<PenaltyAccrual>(list);
        TotalAssiette = list.Where(a => a.Type == PenaltyType.Assiette).Sum(a => a.Amount);
        TotalRecouvrement = list.Where(a => a.Type == PenaltyType.Recouvrement).Sum(a => a.Amount);
        Total = TotalAssiette + TotalRecouvrement;
        History = PenaltyHistory.FromAccruals(list);
    }

    /// <summary>
    /// Accrual entries.
    /// </summary>
    public IReadOnlyCollection<PenaltyAccrual> Accruals { get; }

    /// <summary>
    /// Total assiette penalties.
    /// </summary>
    public decimal TotalAssiette { get; }

    /// <summary>
    /// Total recouvrement penalties.
    /// </summary>
    public decimal TotalRecouvrement { get; }

    /// <summary>
    /// Total penalties.
    /// </summary>
    public decimal Total { get; }

    /// <summary>
    /// Evolution history of penalties.
    /// </summary>
    public PenaltyHistory History { get; }
}
