namespace Core.Bootstrap.AssetTypes;

/// <summary>
/// Registry for asset type definitions.
/// Enables injectable and extensible asset type management.
/// </summary>
public interface IAssetTypeRegistry
{
    /// <summary>
    /// Gets all registered asset type definitions.
    /// </summary>
    IEnumerable<IAssetTypeDefinition> GetDefinitions();

    /// <summary>
    /// Registers a new asset type definition.
    /// </summary>
    void Register(IAssetTypeDefinition definition);

    /// <summary>
    /// Gets a specific definition by key.
    /// </summary>
    IAssetTypeDefinition? Get(string assetTypeKey);
}

/// <summary>
/// Default implementation of IAssetTypeRegistry.
/// </summary>
public sealed class DefaultAssetTypeRegistry : IAssetTypeRegistry
{
    private readonly Dictionary<string, IAssetTypeDefinition> _definitions = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Registers default asset types upon initialization.
    /// </summary>
    public DefaultAssetTypeRegistry()
    {
        // Register default asset types
        Register(new RealEstateAssetTypeDefinition());
        Register(new TransportOperatorAssetTypeDefinition());
        Register(new EconomicActivityAssetTypeDefinition());
        Register(new LegalActAssetTypeDefinition());
        Register(new PersonalIncomeAssetTypeDefinition());
        Register(new PenaltyAssetTypeDefinition());
    }
    /// <summary>
    /// Gets all registered asset type definitions.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<IAssetTypeDefinition> GetDefinitions() => _definitions.Values;

    /// <summary>
    /// Registers a new asset type definition.
    /// </summary>
    /// <param name="definition"></param>
    public void Register(IAssetTypeDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        _definitions[definition.AssetTypeKey] = definition;
    }
    /// <summary>
    /// Gets a specific definition by key.
    /// </summary>
    /// <param name="assetTypeKey"></param>
    /// <returns></returns>
    public IAssetTypeDefinition? Get(string assetTypeKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(assetTypeKey);
        return _definitions.TryGetValue(assetTypeKey, out var definition) ? definition : null;
    }
}
