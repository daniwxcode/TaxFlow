using Core.Domain.Contracts;
using Core.Domain.Enums;
using Core.Domain.Tax.Assets;
using Core.Domain.Tax.Calculation;

namespace Core.Bootstrap.AssetTypes;

public sealed class LegalActAssetTypeDefinition : IAssetTypeDefinition
{
    public string AssetTypeKey => "LEGAL_ACT";
    public string Name => "Legal Act";
    public string Description => "Actes soumis aux droits d'enregistrement";
    public LiquidationMode LiquidationMode => LiquidationMode.Individual;

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
