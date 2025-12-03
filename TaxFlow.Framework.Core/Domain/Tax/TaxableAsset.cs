using Core.Domain.Contracts;
using Core.Domain.Contracts.Abstracts;

using System.Collections.ObjectModel;

namespace Core.Domain.Tax;

public class TaxableAsset: ExtendedTemporalSoftAuditableEntity
{
    public AssetType AssetType { get; private set; }
    public Guid AssetTypeId { get; private set; }

    // EF Core: constructeur sans paramètre required (protégé pour empêcher instanciation hors infra)
    protected TaxableAsset() { }

    // DDD: méthode de création explicite
    public static TaxableAsset Create(AssetType assetType, Collection<ExtendedAttribute> attributes)
    {
        if (assetType is null) throw new ArgumentNullException(nameof(assetType));

        var validationResult = assetType.ValidateAttributes(attributes);
        if(validationResult.Any())
        {
            var errorMessages = string.Join("; ", validationResult.ToArray());
            throw new ArgumentException($"Attributes validation failed: {errorMessages}");
        }
        
        var taxableAsset = new TaxableAsset
        {
            AssetType = assetType,
            AssetTypeId = assetType.Id,
            _attributes = attributes.ToList()
        };
        return taxableAsset;
    }

    
    
}
