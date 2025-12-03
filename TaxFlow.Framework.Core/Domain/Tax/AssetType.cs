using Core.Domain.Contracts;
using Core.Domain.Contracts.Abstracts;
using Core.Domain.Tax.Event;

namespace Core.Domain.Tax;

public class AssetType : SoftAuditableEntity
{
    // EF Core-friendly: backing field pour la collection
    private readonly List<AttributeDefinition> _expectedAttributes = new();

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public IReadOnlyCollection<AttributeDefinition> ExpectedAttributes => _expectedAttributes.AsReadOnly();

    // EF Core: constructeur sans paramètre required (protégé pour empêcher instanciation hors infra)
    protected AssetType() { }

    // DDD: méthode de création explicite
    public static AssetType Create(string name, string? description = null)
    {
        var assetType = new AssetType();
        assetType.Rename(name);
        if (!string.IsNullOrWhiteSpace(description))
        {
            assetType.UpdateDescription(description!);
        }
        return assetType;
    }

    // DDD: intention claire pour renommer
    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Le nom ne doit pas être vide.", nameof(newName));

        Name = newName.Trim();
        // Optionnel: lever un événement de domaine
        QueueDomainEvent(new AssetTypeRenamedDomainEvent(Id, newName));
    }

    // DDD: intention claire pour modifier la description
    public void UpdateDescription(string? newDescription)
    {
        Description = string.IsNullOrWhiteSpace(newDescription) ? null : newDescription.Trim();
        // Optionnel: QueueDomainEvent(new AssetTypeDescriptionUpdatedDomainEvent(Guid, Description));
    }

    // DDD: ajouter un attribut attendu (sans doublon par clé)
    public bool AddExpectedAttribute(AttributeDefinition definition)
    {
        if (definition is null) throw new ArgumentNullException(nameof(definition));
        if (string.IsNullOrWhiteSpace(definition.Key))
            throw new ArgumentException("La clé de l'attribut attendu ne doit pas être vide.", nameof(definition));

        if (HasExpectedAttribute(definition.Key))
            return false;

        _expectedAttributes.Add(definition);
        // Optionnel: QueueDomainEvent(new ExpectedAttributeAddedDomainEvent(Guid, definition.Key));
        return true;
    }

    // DDD: retirer un attribut attendu par instance
    public bool RemoveExpectedAttribute(AttributeDefinition definition)
    {
        if (definition is null) throw new ArgumentNullException(nameof(definition));
        var removed = _expectedAttributes.RemoveAll(a => a.Key.Equals(definition.Key, StringComparison.OrdinalIgnoreCase)) > 0;
        // Optionnel: if (removed) QueueDomainEvent(new ExpectedAttributeRemovedDomainEvent(Guid, definition.Key));
        return removed;
    }

    // DDD: retirer un attribut attendu par clé
    public bool RemoveExpectedAttribute(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("La clé ne doit pas être vide.", nameof(key));

        var removed = _expectedAttributes.RemoveAll(a => a.Key.Equals(key, StringComparison.OrdinalIgnoreCase)) > 0;
        // Optionnel: if (removed) QueueDomainEvent(new ExpectedAttributeRemovedDomainEvent(Guid, key));
        return removed;
    }

    // Vérifier l’existence d’un attribut attendu par clé
    public bool HasExpectedAttribute(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return false;
        return _expectedAttributes.Any(a => a.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
    }

    // Validation basique d’un ensemble d’attributs étendus par rapport aux attentes
    // - Tous les requis doivent être présents
    // - Le type de données doit correspondre (si défini)
    // - La valeur doit être valide selon ExtendedAttribute.IsValidValue()
    public IEnumerable<string> ValidateAttributes(IEnumerable<ExtendedAttribute> attributes)
    {
        if (attributes is null) throw new ArgumentNullException(nameof(attributes));
        var errors = new List<string>();

        var byKey = attributes.ToDictionary(a => a.Key, a => a, StringComparer.OrdinalIgnoreCase);

        foreach (var expected in _expectedAttributes)
        {
            if (expected.IsRequired && !byKey.ContainsKey(expected.Key))
            {
                errors.Add($"Attribut requis manquant: '{expected.Key}'.");
                continue;
            }

            if (byKey.TryGetValue(expected.Key, out var provided))
            {
                // Si le type est contraint, vérifier le DataType
                if (expected.DataType is not null && provided.DataTypeValue != expected.DataType.Value)
                {
                    errors.Add($"Type invalide pour '{expected.Key}': attendu {expected.DataType!.ToString()}, obtenu {provided.DataType.ToString()}.");
                }

                // Vérifier la validité de la valeur au regard du DataType
                if (!provided.IsValidValue())
                {
                    errors.Add($"Valeur invalide pour '{expected.Key}' au format {provided.DataType.ToString()}.");
                }
            }
        }

        return errors;
    }
}
