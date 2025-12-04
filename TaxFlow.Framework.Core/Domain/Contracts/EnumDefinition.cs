using Core.Domain.Contracts.Abstracts;
using Core.Domain.Enums;

using System.Text.RegularExpressions;

namespace Core.Domain.Contracts;

/// <summary>
/// Represents a definition for an enumeration used by attribute definitions.
/// </summary>
public class EnumDefinition : AuditableEntity
{
    /// <summary>
    /// The key identifying this enumeration definition.
    /// </summary>
    public string Key { get; internal set; } = default!;

    /// <summary>
    /// Human-readable label for the enumeration.
    /// </summary>
    public string Label { get; internal set; } = default!;

    /// <summary>
    /// Items that belong to this enumeration.
    /// </summary>
    public ICollection<EnumItem> Items { get; internal set; } = new List<EnumItem>();

    public string BuildLabelRegex()
    {
        var labels = Items
            .Select(i => (i.Label ?? string.Empty).Trim())
            .Where(l => !string.IsNullOrEmpty(l))
            // ordonner par longueur décroissante évite les conflits de préfixe (ex: "A" vs "AB")
            .OrderByDescending(l => l.Length)
            .Select(Regex.Escape);
        return $"^({string.Join("|", labels)})$";
    }

}
