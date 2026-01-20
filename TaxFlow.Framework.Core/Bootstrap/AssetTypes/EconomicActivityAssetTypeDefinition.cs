using Core.Domain.Contracts;
using Core.Domain.Enums;
using Core.Domain.Tax.Assets;
using Core.Domain.Tax.Calculation;

namespace Core.Bootstrap.AssetTypes;

public sealed class EconomicActivityAssetTypeDefinition : IAssetTypeDefinition
{
    public string AssetTypeKey => "ECONOMIC_ACTIVITY";
    public string Name => "Economic Activity";
    public string Description => "Activités économiques soumises à la TPU";
    public LiquidationMode LiquidationMode => LiquidationMode.Individual;

    public AssetType Build()
    {
        var assetType = AssetType.Create(Name, Description, LiquidationMode);
        foreach (var attr in GetAttributes())
            assetType.AddExpectedAttribute(attr);
        foreach (var rule in GetTaxRules())
            assetType.AddTaxRule(rule);
        return assetType;
    }

    private IEnumerable<AttributeDefinition> GetAttributes()
    {
        yield return AttributeDefinition.Create("AnnualTurnover", "Chiffre d'affaires", AttributeDataType.Number, true);
        yield return AttributeDefinition.Create(new EnumDefinition
        {
            Key = "ActivityNature",
            Label = "Nature de l'activité",
            Items =
            {
                new EnumItem { Code = "COM", Label = "Commerce", Order = 1 },
                new EnumItem { Code = "SRV", Label = "Services", Order = 2 },
                new EnumItem { Code = "ART", Label = "Artisan", Order = 3 },
                new EnumItem { Code = "AMB", Label = "Ambulant", Order = 4 },
                new EnumItem { Code = "ELV", Label = "Eleveur", Order = 5 }
            }
        });
        yield return AttributeDefinition.Create("UsesMechanicalMeans", "Utilise des moyens mécaniques", AttributeDataType.Boolean);
        yield return AttributeDefinition.Create(new EnumDefinition
        {
            Key = "LocationCategory",
            Label = "Catégorie de localisation",
            Items =
            {
                new EnumItem { Code = "URB", Label = "Urbain", Order = 1 },
                new EnumItem { Code = "SEMI", Label = "Semi-urbain", Order = 2 },
                new EnumItem { Code = "RUR", Label = "Rural", Order = 3 }
            }
        }, false);
        yield return AttributeDefinition.Create("HasFranchise", "Bénéficie d'une franchise", AttributeDataType.Boolean);
    }

    private IEnumerable<TaxRule> GetTaxRules()
    {
        yield return new TaxRule
        {
            Key = "TPU_ECO",
            Label = "TPU – Activités économiques",
            Description = "Barème forfaitaire commerce/services et montants différenciés artisans/ambulants.",
            Expression = """
            [HasFranchise]?0:
            (
                (([ActivityNatureCode]=="COM" and [AnnualTurnover]<=2500000)?10000:0) +
                (([ActivityNatureCode]=="COM" and [AnnualTurnover]>2500000 and [AnnualTurnover]<=5000000)?40000:0) +
                (([ActivityNatureCode]=="COM" and [AnnualTurnover]>5000000 and [AnnualTurnover]<=10000000)?115000:0) +
                (([ActivityNatureCode]=="COM" and [AnnualTurnover]>10000000 and [AnnualTurnover]<=15000000)?190000:0) +
                (([ActivityNatureCode]=="COM" and [AnnualTurnover]>15000000 and [AnnualTurnover]<=20000000)?265000:0) +
                (([ActivityNatureCode]=="COM" and [AnnualTurnover]>20000000 and [AnnualTurnover]<=25000000)?340000:0) +
                (([ActivityNatureCode]=="COM" and [AnnualTurnover]>25000000 and [AnnualTurnover]<=30000000)?415000:0) +
                (([ActivityNatureCode]=="COM" and [AnnualTurnover]>30000000)?500000:0) +
                (([ActivityNatureCode]=="SRV" and [AnnualTurnover]<=2500000)?10000*1.35:0) +
                (([ActivityNatureCode]=="SRV" and [AnnualTurnover]>2500000 and [AnnualTurnover]<=5000000)?40000*1.35:0) +
                (([ActivityNatureCode]=="SRV" and [AnnualTurnover]>5000000 and [AnnualTurnover]<=10000000)?115000*1.35:0) +
                (([ActivityNatureCode]=="SRV" and [AnnualTurnover]>10000000 and [AnnualTurnover]<=15000000)?190000*1.35:0) +
                (([ActivityNatureCode]=="SRV" and [AnnualTurnover]>15000000 and [AnnualTurnover]<=20000000)?265000*1.35:0) +
                (([ActivityNatureCode]=="SRV" and [AnnualTurnover]>20000000 and [AnnualTurnover]<=25000000)?340000*1.35:0) +
                (([ActivityNatureCode]=="SRV" and [AnnualTurnover]>25000000 and [AnnualTurnover]<=30000000)?415000*1.35:0) +
                (([ActivityNatureCode]=="SRV" and [AnnualTurnover]>30000000)?500000*1.35:0) +
                ([ActivityNatureCode]=="ART"?
                    (([UsesMechanicalMeans]?30000:15000)*
                     ([LocationCategoryCode]=="URB"?1:([LocationCategoryCode]=="SEMI"?0.85:0.65)))
                :0) +
                ([ActivityNatureCode]=="AMB"?
                    (10000*([LocationCategoryCode]=="URB"?1:([LocationCategoryCode]=="SEMI"?0.9:0.7)))
                :0) +
                ([ActivityNatureCode]=="ELV"?
                    (8000*([LocationCategoryCode]=="URB"?1:([LocationCategoryCode]=="SEMI"?0.85:0.6)))
                :0)
            )
            """
        };
    }
}
