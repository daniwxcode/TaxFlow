using Core.Domain.Tax.Penalties;

namespace Core.Domain.Tax.Obligations;

/// <summary>
/// Represents a declaration/filing deadline with associated penalty rules.
/// </summary>
public sealed class DeclarationDeadline : TaxDeadline
{
    /// <inheritdoc />
    public override DeadlineType Type => DeadlineType.Declaration;

    /// <summary>
    /// Penalty definition applied when the declaration deadline is missed.
    /// This typically represents "pénalité d'assiette" for late filing.
    /// </summary>
    public PenaltyDefinition? PenaltyDefinition { get; private set; }

    /// <summary>
    /// Creates a new declaration deadline.
    /// </summary>
    /// <param name="key">Unique key for this deadline.</param>
    /// <param name="label">Human-readable label.</param>
    /// <param name="dueDate">Due date for the declaration.</param>
    /// <param name="graceDays">Grace period in days.</param>
    public static DeclarationDeadline Create(
        string key,
        string label,
        DateTimeOffset dueDate,
        int graceDays = 0)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(label);

        return new DeclarationDeadline
        {
            Key = key.Trim(),
            Label = label.Trim(),
            DueDate = dueDate,
            GraceDays = graceDays
        };
    }

    /// <summary>
    /// Configures the penalty definition for late declaration.
    /// </summary>
    /// <param name="definition">Penalty definition to apply.</param>
    public DeclarationDeadline WithPenalty(PenaltyDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        PenaltyDefinition = definition;
        return this;
    }

    /// <summary>
    /// Checks if a penalty applies for this deadline as of the given date.
    /// </summary>
    public bool HasPenalty(DateTimeOffset asOf) => PenaltyDefinition is not null && IsOverdue(asOf);
}
