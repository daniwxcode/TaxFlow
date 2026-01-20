using Core.Domain.Contracts;
using Core.Domain.Enums;
using Core.Domain.Tax.Assets;
using Core.Domain.Tax.Calculation;

namespace Core.Bootstrap.AssetTypes;

public sealed class PenaltyAssetTypeDefinition : IAssetTypeDefinition
{
    public string AssetTypeKey => "PENALTY";
    public string Name => "Recovery Penalties";
    public string Description => "Pénalité de recouvrement (PENAR)";
    public LiquidationMode LiquidationMode => LiquidationMode.Individual;

    public AssetType Build()
    {
        var assetType = AssetType.Create(Name, Description, LiquidationMode);

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
