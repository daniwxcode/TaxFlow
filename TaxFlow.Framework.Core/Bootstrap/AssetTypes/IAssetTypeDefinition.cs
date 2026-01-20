using Core.Domain.Enums;
using Core.Domain.Tax.Assets;

namespace Core.Bootstrap.AssetTypes;

/// <summary>
/// Contract for defining an asset type and its configuration.
/// Enables dependency injection and extensibility.
/// </summary>
public interface IAssetTypeDefinition
{
    /// <summary>
    /// Unique identifier for this asset type.
    /// </summary>
    string AssetTypeKey { get; }

    /// <summary>
    /// Human-readable name of the asset type.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Description of the asset type.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Liquidation mode for this asset type (Individual or Grouped).
    /// </summary>
    LiquidationMode LiquidationMode { get; }

    /// <summary>
    /// Builds the asset type with its attributes and tax rules.
    /// </summary>
    /// <returns>Fully configured AssetType.</returns>
    AssetType Build();
}
