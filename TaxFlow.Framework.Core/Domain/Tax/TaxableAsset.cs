using Core.Domain.Contracts;
using Core.Domain.Contracts.Abstracts;

using System.Collections.ObjectModel;

namespace Core.Domain.Tax;

/// <summary>
/// Represents an asset subject to taxation. Contains a reference to its asset type and a collection of extended attributes.
/// </summary>
public class TaxableAsset: ExtendedTemporalSoftAuditableEntity
{
    /// <summary>
    /// The asset type describing the expected attributes and rules for this asset.
    /// </summary>
    public AssetType AssetType { get; private set; }

    /// <summary>
    /// Foreign key identifier of the asset type.
    /// </summary>
    public Guid AssetTypeId { get; private set; }

    /// <summary>
    /// Protected parameterless constructor for EF Core and infrastructure.
    /// </summary>
    protected TaxableAsset() { }

    /// <summary>
    /// Factory method to create a new <see cref="TaxableAsset"/> instance while enforcing attribute validation against the provided <see cref="AssetType"/>.
    /// </summary>
    /// <param name="assetType">The asset type to associate with this asset.</param>
    /// <param name="attributes">Collection of extended attributes for the asset.</param>
    /// <returns>A new <see cref="TaxableAsset"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="assetType"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when attribute validation fails.</exception>
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
