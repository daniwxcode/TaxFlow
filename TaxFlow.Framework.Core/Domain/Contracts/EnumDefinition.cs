using Core.Domain.Contracts.Abstracts;
using Core.Domain.Enums;
using Core.Domain.Localization;

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

    /// <summary>
    /// Creates a new enum item.
    /// </summary>
    public static EnumItem CreateItem(string code, string label, int order = 0)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(ExceptionMessages.EnumItemCodeCannotBeEmpty.Format(), nameof(code));
        if (string.IsNullOrWhiteSpace(label))
            throw new ArgumentException(ExceptionMessages.EnumItemLabelCannotBeEmpty.Format(), nameof(label));

        return new EnumItem
        {
            Code = code.Trim(),
            Label = label.Trim(),
            Order = order
        };
    }

    /// <summary>
    /// Creates a new enum definition with items.
    /// </summary>
    public static EnumDefinition Create(string key, string label, IEnumerable<EnumItem> items)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException(ExceptionMessages.EnumDefinitionKeyCannotBeEmpty.Format(), nameof(key));
        if (string.IsNullOrWhiteSpace(label))
            throw new ArgumentException(ExceptionMessages.EnumDefinitionLabelCannotBeEmpty.Format(), nameof(label));
        ArgumentNullException.ThrowIfNull(items);

        var definition = new EnumDefinition
        {
            Key = key.Trim(),
            Label = label.Trim()
        };

        foreach (var item in items)
        {
            ArgumentNullException.ThrowIfNull(item);
            item.EnumDefinition = definition;
            definition.Items.Add(item);
        }

        return definition;
    }

    /// <summary>
    /// Construit une regex basée sur les labels des items d'énumération.
    /// À utiliser pour valider les valeurs d'attribut de type Enum (affichage par label).
    /// </summary>
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

    /// <summary>
    /// Try to resolve the provided value into a label by matching either code or label (case-insensitive).
    /// </summary>
    /// <param name="value">Input value to match (code or label).</param>
    /// <param name="label">Resolved label when match succeeds.</param>
    /// <returns>True if a matching item was found; otherwise false.</returns>
    public bool TryGetLabel(string? value, out string label)
    {
        label = string.Empty;
        if (string.IsNullOrWhiteSpace(value)) return false;

        var normalized = value.Trim();
        var match = Items.FirstOrDefault(i =>
            string.Equals(i.Code, normalized, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(i.Label, normalized, StringComparison.OrdinalIgnoreCase));

        if (match is null) return false;
        label = match.Label ?? string.Empty;
        return !string.IsNullOrWhiteSpace(label);
    }

    /// <summary>
    /// Try to resolve the provided value into a code by matching either code or label (case-insensitive).
    /// </summary>
    /// <param name="value">Input value to match (code or label).</param>
    /// <param name="code">Resolved code when match succeeds.</param>
    /// <returns>True if a matching item was found; otherwise false.</returns>
    public bool TryGetCode(string? value, out string code)
    {
        code = string.Empty;
        if (string.IsNullOrWhiteSpace(value)) return false;

        var normalized = value.Trim();
        var match = Items.FirstOrDefault(i =>
            string.Equals(i.Code, normalized, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(i.Label, normalized, StringComparison.OrdinalIgnoreCase));

        if (match is null) return false;
        code = match.Code ?? string.Empty;
        return !string.IsNullOrWhiteSpace(code);
    }
}
