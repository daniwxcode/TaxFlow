using Core.Domain.Contracts;
using Core.Domain.Enums;
using Core.Domain.Tax.Assets;
using Core.Domain.Tax.Calculation;

namespace Core.Bootstrap.AssetTypes;
/// <summary>
/// Penalty asset type definition.
/// </summary>
public sealed class PenaltyAssetTypeDefinition : IAssetTypeDefinition
{
    /// <summary>
    /// Assets type key for Recovery Penalties.
    /// </summary>
    public string AssetTypeKey => "PENALTY";
    /// <summary>
    /// Name of the asset type.
    /// </summary>
    public string Name => "Recovery Penalties";
    /// <summary>
    /// Description of the asset type.
    /// </summary>
    public string Description => "Pénalité de recouvrement (PENAR)";
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
        AssetType assetType = AssetType.Create(Name, Description, LiquidationMode);

        assetType.AddExpectedAttribute(AttributeDefinition.Create(
            "OutstandingTaxAmount", "Montant en souffrance", AttributeDataType.Number, true));

        assetType.AddTaxRule(new TaxRule
        {
            Key = "PENAR",
            Label = "PÉNALITÉ DE RECOUVREMENT",
            Description = "Taux proportionnel de 10 % avec plancher 1 000.",
            Expression = """
            (([OutstandingTaxAmount]*0.10)<1000?1000:[OutstandingTaxAmount]*0.10)
            """
        });

        return assetType;
    }
}
