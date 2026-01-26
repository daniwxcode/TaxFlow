using System;
using System.Linq;

namespace Core.Domain.Tax.Penalties;

/// <summary>
/// Defines penalty calculation rules for assiette and recouvrement.
/// </summary>
public sealed class PenaltyPolicy
{
    private readonly Dictionary<PenaltyType, PenaltyDefinition> _definitions = new();

    /// <summary>
    /// Number of days in the base period for proration (default 365).
    /// </summary>
    public int DaysInYear { get; init; } = 365;

    /// <summary>
    /// Minimum acceptable amount for a penalty line (lines below are skipped).
    /// </summary>
    public decimal MinimumLineAmount { get; init; } = 0m;

    /// <summary>
    /// Registered penalty definitions.
    /// </summary>
    public IReadOnlyCollection<PenaltyDefinition> Definitions => _definitions.Values.ToList().AsReadOnly();

    /// <summary>
    /// Add or update a penalty definition.
    /// </summary>
    /// <param name="definition">Definition to add or update.</param>
    public void AddOrUpdateDefinition(PenaltyDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        _definitions[definition.Type] = definition;
    }

    /// <summary>
    /// Get a penalty definition by type.
    /// </summary>
    /// <param name="type">Penalty type.</param>
    /// <returns>Definition or null.</returns>
    public PenaltyDefinition? GetDefinition(PenaltyType type)
        => _definitions.TryGetValue(type, out var def) ? def : null;

    /// <summary>
    /// Get penalty definitions that are triggered by the given event.
    /// </summary>
    /// <param name="triggerEvent">Trigger event.</param>
    /// <returns>Matching definitions.</returns>
    public IReadOnlyCollection<PenaltyDefinition> GetDefinitionsForEvent(PenaltyTriggerEvent triggerEvent)
    {
        return _definitions.Values
            .Where(d => d.TriggerEvent == PenaltyTriggerEvent.Any || d.TriggerEvent.HasFlag(triggerEvent))
            .ToList()
            .AsReadOnly();
    }

    /// <summary>
    /// Validate policy values.
    /// </summary>
    public void Validate()
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(DaysInYear, nameof(DaysInYear));
        ArgumentOutOfRangeException.ThrowIfNegative(MinimumLineAmount, nameof(MinimumLineAmount));
        foreach (var def in _definitions.Values)
        {
            def.Validate();
        }
    }
}
