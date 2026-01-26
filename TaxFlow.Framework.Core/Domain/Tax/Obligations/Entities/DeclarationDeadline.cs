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
    /// Type of declaration (initial, corrective, null, etc.).
    /// </summary>
    public DeclarationType DeclarationType { get; init; } = DeclarationType.Initial;

    /// <summary>
    /// Whether this declaration requires supporting documents.
    /// </summary>
    public bool RequiresDocuments { get; init; } = false;

    /// <summary>
    /// Form or document reference for this declaration.
    /// </summary>
    public string? FormReference { get; init; }

    /// <summary>
    /// Creates a new declaration deadline.
    /// </summary>
    /// <param name="key">Unique key for this deadline.</param>
    /// <param name="label">Human-readable label.</param>
    /// <param name="dueDate">Due date for the declaration.</param>
    /// <param name="gracePeriod">Grace period before penalties apply.</param>
    public static DeclarationDeadline Create(
        string key,
        string label,
        DateTimeOffset dueDate,
        Duration gracePeriod = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(label);

        return new DeclarationDeadline
        {
            Key = key.Trim(),
            Label = label.Trim(),
            DueDate = dueDate,
            GracePeriod = gracePeriod
        };
    }

    /// <summary>
    /// Creates a new declaration deadline with full configuration.
    /// </summary>
    public static DeclarationDeadline Create(
        string key,
        string label,
        DateTimeOffset dueDate,
        DeadlinePeriodicity periodicity,
        TaxRegime regime = TaxRegime.General,
        Duration gracePeriod = default,
        int order = 1)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(label);

        return new DeclarationDeadline
        {
            Key = key.Trim(),
            Label = label.Trim(),
            DueDate = dueDate,
            GracePeriod = gracePeriod,
            Periodicity = periodicity,
            Regime = regime,
            Order = order
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
    /// Adds a legal reference to this declaration deadline.
    /// </summary>
    public new DeclarationDeadline AddLegalReference(LegalReference reference)
    {
        base.AddLegalReference(reference);
        return this;
    }

    /// <summary>
    /// Sets the declaration type.
    /// </summary>
    public DeclarationDeadline WithDeclarationType(DeclarationType type)
    {
        return new DeclarationDeadline
        {
            Key = Key,
            Label = Label,
            DueDate = DueDate,
            GracePeriod = GracePeriod,
            Periodicity = Periodicity,
            Regime = Regime,
            Order = Order,
            Description = Description,
            Enabled = Enabled,
            DeclarationType = type,
            RequiresDocuments = RequiresDocuments,
            FormReference = FormReference,
            ConditionExpression = ConditionExpression,
            FiscalYear = FiscalYear,
            Period = Period,
            PenaltyDefinition = PenaltyDefinition
        };
    }

    /// <summary>
    /// Sets the form reference.
    /// </summary>
    public DeclarationDeadline WithFormReference(string formReference)
    {
        return new DeclarationDeadline
        {
            Key = Key,
            Label = Label,
            DueDate = DueDate,
            GracePeriod = GracePeriod,
            Periodicity = Periodicity,
            Regime = Regime,
            Order = Order,
            Description = Description,
            Enabled = Enabled,
            DeclarationType = DeclarationType,
            RequiresDocuments = RequiresDocuments,
            FormReference = formReference?.Trim(),
            ConditionExpression = ConditionExpression,
            FiscalYear = FiscalYear,
            Period = Period,
            PenaltyDefinition = PenaltyDefinition
        };
    }

    /// <summary>
    /// Checks if a penalty applies for this deadline as of the given date.
    /// </summary>
    public bool HasPenalty(DateTimeOffset asOf) => PenaltyDefinition is not null && IsOverdue(asOf);
}

/// <summary>
/// Type of declaration.
/// </summary>
public enum DeclarationType
{
    /// <summary>
    /// Initial/original declaration.
    /// </summary>
    Initial = 0,

    /// <summary>
    /// Corrective/amended declaration.
    /// </summary>
    Corrective = 1,

    /// <summary>
    /// Null declaration (no activity/no tax due).
    /// </summary>
    Null = 2,

    /// <summary>
    /// Provisional declaration (subject to adjustment).
    /// </summary>
    Provisional = 3,

    /// <summary>
    /// Final/definitive declaration.
    /// </summary>
    Final = 4,

    /// <summary>
    /// Supplementary declaration.
    /// </summary>
    Supplementary = 5
}
