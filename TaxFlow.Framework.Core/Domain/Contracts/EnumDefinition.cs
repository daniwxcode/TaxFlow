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

    /// <summary>
    /// Construit une regex basée sur les codes des items d'énumération.
    /// À utiliser pour valider les valeurs d'attribut de type Enum (stockage par code).
    /// </summary>
    public string BuildCodeRegex()
    {
        var codes = Items
            .Select(i => (i.Code ?? string.Empty).Trim())
            .Where(c => !string.IsNullOrEmpty(c))
            .OrderByDescending(c => c.Length)
            .Select(Regex.Escape);
        return $"^({string.Join("|", codes)})$";
    }
}
