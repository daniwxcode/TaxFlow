using Core.Domain.Contracts;
using Core.Domain.Enums;
using Core.Domain.Tax.Assets;
using Core.Domain.Tax.Calculation;

namespace Core.Bootstrap.AssetTypes;

/// <summary>
/// Legal Act asset type definition.
/// </summary>
public sealed class LegalActAssetTypeDefinition : IAssetTypeDefinition
{
    /// <summary>
    /// Gets the unique identifier for this asset type.
    /// </summary>
    public string AssetTypeKey => "LEGAL_ACT";
    /// <summary>
    /// Gets the human-readable name of the asset type.
    /// </summary>
    public string Name => "Legal Act";
    /// <summary>
    /// Gets the description of the asset type.
    /// </summary>
    public string Description => "Actes soumis aux droits d'enregistrement";
    /// <summary>
    /// Gets the liquidation mode for this asset type.
    /// </summary>
    public LiquidationMode LiquidationMode => LiquidationMode.Individual;

    /// <summary>
    /// Builds the asset type definition.
    /// </summary>
    /// <returns></returns>
    public AssetType Build()
    {
        var assetType = AssetType.Create(Name, Description, LiquidationMode);
        assetType.AddExpectedAttribute(AttributeDefinition.Create(
            "ContractAmount", "Montant contractuel", AttributeDataType.Number, true));
        
        assetType.AddTaxRule(new TaxRule
        {
            Key = "ENR",
            Label = "DROITS D'ENREGISTREMENT",
            Description = "Taxe proportionnelle de 2 % (actuellement inactive).",
            Enabled = false,
            Expression = "[ContractAmount]*2/100"
        });

        return assetType;
    }
}
