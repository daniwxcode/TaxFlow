using Core.Domain.Contracts.Abstracts;

namespace Core.Domain.Contracts;

/// <summary>
/// Represents one item inside an EnumDefinition.
/// </summary>
public class EnumItem : AuditableEntity
{
    /// <summary>
    /// The code used to identify the enum item (stored value).
    /// </summary>
    public string Code { get; internal set; } = default!;

    /// <summary>
    /// Human-readable label for the item.
    /// </summary>
    public string Label { get; internal set; } = default!;

    /// <summary>
    /// Order for presentation or sorting within the enum.
    /// </summary>
    public int Order { get; internal set; }

    /// <summary>
    /// Foreign key to the owning <see cref="EnumDefinition"/> - used by persistence.
    /// </summary>
    public int EnumDefinitionId { get; internal set; }

    /// <summary>
    /// Navigation property to the owning <see cref="EnumDefinition"/>.
    /// </summary>
    public EnumDefinition EnumDefinition { get; internal set; } = default!;
}
