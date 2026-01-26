using Core.Domain.Contracts;
using Core.Domain.Enums;
using Core.Domain.Tax.Assets;
using Core.Domain.Tax.Calculation;
using Core.Domain.Tax.Obligations;

namespace Core.Bootstrap.AssetTypes;

/// <summary>
/// Asset type definition for Real Estate (Immobilier).
/// Handles tax housing, land tax, and rental income tax.
/// </summary>
public sealed class RealEstateAssetTypeDefinition : IAssetTypeDefinition
{
    /// <summary>
    /// Gets the unique identifier for this asset type.
    /// </summary>
    public string AssetTypeKey => "REAL_ESTATE";
    /// <summary>
    /// Gets the human-readable name of the asset type.
    /// </summary>
    public string Name => "Real Estate";
    /// <summary>
    /// Gets the description of the asset type.
    /// </summary>
    public string Description => "Propriété Immobilière Maison et Terrain";
    /// <summary>
    /// Gets the liquidation mode for this asset type.
    /// </summary>
    public LiquidationMode LiquidationMode => LiquidationMode.Individual;
    /// <summary>
    /// Builds the asset type with its attributes and tax rules.
    /// </summary>
    /// <returns></returns>
    public AssetType Build()
    {
        AssetType assetType = AssetType.Create(Name, Description, LiquidationMode);

        foreach (AttributeDefinition attr in GetAttributes())
        {
            assetType.AddExpectedAttribute(attr);
        }

        foreach (TaxRule rule in GetTaxRules())
        {
            assetType.AddTaxRule(rule);
        }

        return assetType;
    }

    private IEnumerable<AttributeDefinition> GetAttributes()
    {
        yield return AttributeDefinition.Create("ResidualValue", "Valeur vénale", AttributeDataType.Number, true);
        yield return AttributeDefinition.Create("Situation", "Situation", AttributeDataType.String);
        yield return AttributeDefinition.Create(new EnumDefinition
        {
            Key = "RealEstateType",
            Label = "Type de Propriété",
            Items =
            {
                new EnumItem { Code = "PB", Label = "Propriété Bâtie", Order = 1 },
                new EnumItem { Code = "PNB", Label = "Propriété Non Bâtie", Order = 2 }
            }
        });
        yield return AttributeDefinition.Create(new EnumDefinition
        {
            Key = "ResidenceType",
            Label = "Type de Résidence",
            Items =
            {
                new EnumItem { Code = "RP", Label = "Résidence Principale", Order = 1 },
                new EnumItem { Code = "RS", Label = "Résidence Secondaire", Order = 2 }
            }
        }, false);
        yield return AttributeDefinition.Create(new EnumDefinition
        {
            Key = "RealEstateUsage",
            Label = "Usage d'un bien immobilier",
            Items =
            {
                new EnumItem { Code = "RES", Label = "Résidentiel", Order = 1 },
                new EnumItem { Code = "COM", Label = "Location", Order = 2 }
            }
        }, false);
        yield return AttributeDefinition.Create(new EnumDefinition
        {
            Key = "RealEstateCategory",
            Label = "Type d'Habitat",
            Items =
            {
                new EnumItem { Code = "STUDIO", Label = "Appartement 1 pièce (studio)", Order = 1 },
                new EnumItem { Code = "CON", Label = "Concession", Order = 2 },
                new EnumItem { Code = "APT2", Label = "Appartement 2 pièces", Order = 3 },
                new EnumItem { Code = "APT3P", Label = "Appartement 3 pièces et plus", Order = 4 },
                new EnumItem { Code = "VILLA", Label = "Villa / concession unique", Order = 5 },
                new EnumItem { Code = "HOUSE_R1", Label = "Maison R+1", Order = 6 },
                new EnumItem { Code = "HOUSE_R2", Label = "Maison R+2", Order = 7 },
                new EnumItem { Code = "HOUSE_R3", Label = "Maison R+3 et plus", Order = 8 },
                new EnumItem { Code = "ETG600", Label = "Étage > 600 m²", Order = 9 }
            }
        }, false);
        yield return AttributeDefinition.Create(new EnumDefinition
        {
            Key = "RealEstateOwnerShip",
            Label = "Droit de Propriété",
            Items =
            {
                new EnumItem { Code = "OWNER", Label = "Propriétaire" },
                new EnumItem { Code = "RENT", Label = "Locataire" }
            }
        }, false);
        yield return AttributeDefinition.Create(new EnumDefinition
        {
            Key = "ResidenceStatus",
            Label = "Statut de la Résidence",
            Items =
            {
                new EnumItem { Code = "NEW", Label = "Nouvelle", Order = 1 },
                new EnumItem { Code = "OLD", Label = "Ancienne", Order = 2 }
            }
        }, false);
        yield return AttributeDefinition.Create("AcquisitionDate", "Date d'acquisition", AttributeDataType.Date);
        yield return AttributeDefinition.Create("BuildingCompletionDate", "Date de fin des Travaux", AttributeDataType.Date);
        yield return AttributeDefinition.Create("LocativeValue", "Valeur locative annuelle", AttributeDataType.Number);
        yield return AttributeDefinition.Create("NetRentalIncome", "Revenu net foncier", AttributeDataType.Number);
        yield return AttributeDefinition.Create("AnnualRent", "Loyer perçu", AttributeDataType.Number);
    }

    private IEnumerable<TaxRule> GetTaxRules()
    {
        yield return new TaxRule
        {
            Key = "TH",
            Label = "TAXE D'HABITATION",
            Description = "Barème forfaitaire selon la catégorie d'habitation.",
            ValidFrom = default,
            Expression = """
            ([RealEstateCategoryCode]=="STUDIO"?2000:0) +
            ([RealEstateCategoryCode]=="CON"?4000:0) +
            ([RealEstateCategoryCode]=="APT2"?6000:0) +
            ([RealEstateCategoryCode]=="APT3P"?9000:0) +
            ([RealEstateCategoryCode]=="VILLA"?30000:0) +
            ([RealEstateCategoryCode]=="HOUSE_R1"?40000:0) +
            ([RealEstateCategoryCode]=="HOUSE_R2"?75000:0) +
            ([RealEstateCategoryCode]=="HOUSE_R3"?100000:0) +
            ([RealEstateCategoryCode]=="ETG600"?100000:0)
            """
        };

        yield return new TaxRule
        {
            Key = "TFPB",
            Label = "TAXE FONCIÈRE SUR PROPRIÉTÉS BÂTIES",
            Description = "Taux proportionnel annuel de 7,5 % sur la valeur locative.",
            ValidFrom = default,
            Expression = """
            [RealEstateTypeCode]=="PB"?[LocativeValue]*7.5/100:0
            """
        };

        yield return new TaxRule
        {
            Key = "TFPNB",
            Label = "TAXE FONCIÈRE SUR PROPRIÉtÉS NON BÂTIES",
            Description = "Taux proportionnel annuel de 0,5 % sur la valeur vénale du terrain.",
            ValidFrom = default,
            Expression = """
            [RealEstateTypeCode]=="PNB"?[ResidualValue]*0.5/100:0
            """
        };

        yield return new TaxRule
        {
            Key = "IRF",
            Label = "IMPÔT SUR LE REVENU FONCIER",
            Description = "Barème progressif IRPP appliqué au revenu net foncier.",
            Expression = BuildIrppScaleExpression("NetRentalIncome")
        };

        yield return new TaxRule
        {
            Key = "RSL",
            Label = "RETENUE SUR LOYER",
            Description = "Taxe proportionnelle de 8,75 % sur les loyers perçus.",
            Expression = """
            [AnnualRent]*8.75/100
            """
        };
    }

    private static string BuildIrppScaleExpression(string variableName) =>
        $"""
        (
            (([{variableName}]>900000?([{variableName}]<3000000?[{variableName}]:3000000):900000)-900000)*0.10 +
            (([{variableName}]>3000000?([{variableName}]<9000000?[{variableName}]:9000000):3000000)-3000000)*0.15 +
            (([{variableName}]>9000000?([{variableName}]<12000000?[{variableName}]:12000000):9000000)-9000000)*0.20 +
            (([{variableName}]>12000000?([{variableName}]<15000000?[{variableName}]:15000000):12000000)-12000000)*0.25 +
            (([{variableName}]>15000000?([{variableName}]<20000000?[{variableName}]:20000000):15000000)-15000000)*0.30 +
            (([{variableName}]>20000000?[{variableName}]:20000000)-20000000)*0.35
        )
        """;
}
