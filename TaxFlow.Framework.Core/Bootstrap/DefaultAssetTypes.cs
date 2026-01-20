using Core.Bootstrap.AssetTypes;
using Core.Domain.Tax.Assets;

namespace Core.Bootstrap;

/// <summary>
/// Provides default asset types and their configurations using the registry pattern.
/// This acts as a seed provider for initial data.
/// </summary>
public static class DefaultAssetTypes
{
    /// <summary>
    /// Gets the initial data for asset types using the default registry.
    /// </summary>
    public static IEnumerable<AssetType> InitialData()
    {
        var registry = new DefaultAssetTypeRegistry();
        return registry.GetDefinitions().Select(def => def.Build());
    }

    /// <summary>
    /// Gets the asset type registry for injectable use.
    /// </summary>
    public static IAssetTypeRegistry GetRegistry() => new DefaultAssetTypeRegistry();
}
