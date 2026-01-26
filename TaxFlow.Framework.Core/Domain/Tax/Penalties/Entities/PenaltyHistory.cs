using System.Collections.ObjectModel;

namespace Core.Domain.Tax.Penalties;

/// <summary>
/// Represents the evolution history of penalties.
/// </summary>
public sealed class PenaltyHistory
{
    /// <summary>
    /// Create a penalty history.
    /// </summary>
    /// <param name="entries">History entries.</param>
    public PenaltyHistory(IEnumerable<PenaltyHistoryEntry> entries)
    {
        var list = (entries ?? Array.Empty<PenaltyHistoryEntry>()).ToList();
        Entries = new ReadOnlyCollection<PenaltyHistoryEntry>(list);
    }

    /// <summary>
    /// Chronological entries of penalty evolution.
    /// </summary>
    public IReadOnlyCollection<PenaltyHistoryEntry> Entries { get; }

    /// <summary>
    /// Build a history from accrual lines.
    /// </summary>
    /// <param name="accruals">Penalty accruals.</param>
    public static PenaltyHistory FromAccruals(IEnumerable<PenaltyAccrual> accruals)
    {
        var ordered = (accruals ?? Array.Empty<PenaltyAccrual>())
            .OrderBy(a => a.PeriodEnd)
            .ThenBy(a => a.AsOf)
            .ToList();

        List<PenaltyHistoryEntry> entries = new (ordered.Count);
        decimal totalAssiette = 0m;
        decimal totalRecouvrement = 0m;

        foreach (var a in ordered)
        {
            if (a.Type == PenaltyType.Assiette)
            {
                totalAssiette += a.Amount;
            }

            if (a.Type == PenaltyType.Recouvrement)
            {
                totalRecouvrement += a.Amount;
            }

            entries.Add(new PenaltyHistoryEntry(
                a.PeriodEnd,
                a,
                totalAssiette,
                totalRecouvrement,
                totalAssiette + totalRecouvrement));
        }

        return new PenaltyHistory(entries);
    }
}

/// <summary>
/// Represents a single evolution step for penalties.
/// </summary>
public sealed class PenaltyHistoryEntry
{
    /// <summary>
    /// Create a history entry.
    /// </summary>
    /// <param name="date">Entry date.</param>
    /// <param name="line">Penalty line.</param>
    /// <param name="totalAssiette">Cumulative assiette total.</param>
    /// <param name="totalRecouvrement">Cumulative recouvrement total.</param>
    /// <param name="total">Cumulative total.</param>
    public PenaltyHistoryEntry(DateTimeOffset date, PenaltyAccrual line, decimal totalAssiette, decimal totalRecouvrement, decimal total)
    {
        Date = date;
        Line = line;
        TotalAssiette = totalAssiette;
        TotalRecouvrement = totalRecouvrement;
        Total = total;
    }

    /// <summary>
    /// Entry date (period end).
    /// </summary>
    public DateTimeOffset Date { get; }

    /// <summary>
    /// Line that produced this evolution step.
    /// </summary>
    public PenaltyAccrual Line { get; }

    /// <summary>
    /// Cumulative assiette penalties.
    /// </summary>
    public decimal TotalAssiette { get; }

    /// <summary>
    /// Cumulative recouvrement penalties.
    /// </summary>
    public decimal TotalRecouvrement { get; }

    /// <summary>
    /// Cumulative total penalties.
    /// </summary>
    public decimal Total { get; }
}
