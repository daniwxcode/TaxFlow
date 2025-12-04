using Core.Domain.Contracts;
using Core.Domain.Enums;

namespace Core.Domain.Tax.Bootstrap;

public static class DefaultAssetTypes
{
    public static IEnumerable<TaxRule> RealEstateTaxRules()
    {
        yield return new TaxRule()
        {
            Key = "TFNB",
            Label = "TAXE FONCIERE SUR PROPRIETE NON BATIE",
            ValidFrom = default,
            Expression = """
            ([RealEstateType]=="Propriété Bâtie"||[RealEstateUsage]=="Location")?0:[ResidualValue]*0.5/100
            """
        };
        yield return new TaxRule()
        {
            Key = "TFB",
            Label = "TAXE FONCIERE SUR PROPRIETE BATIE",
            ValidFrom = default,
            Expression = """
            ([RealEstateType]=="Propriété Bâtie"||[RealEstateUsage]=="Location")?[ResidualValue]*0.75/100:0
            """
        };
    }
    public static IEnumerable<AttributeDefinition> RealEstateAttributes()
    {
        yield return AttributeDefinition.Create("ResidualValue", "Valeur Venale", AttributeDataType.Number, true);
        yield return AttributeDefinition.Create("Situation", "Situation", AttributeDataType.String);
        yield return AttributeDefinition.Create(new EnumDefinition
        {
            Key = "RealEstateType",
            Label = "Type de Propriété",
            Items =
            {
                new EnumItem{Code="PB",Label="Propriété Bâtie",Order=1},
                new EnumItem{Code="PNB",Label="Propriété Non Bâtie", Order=2}
            }

        });
        yield return AttributeDefinition.Create(new EnumDefinition
        {
            Key = "ResidenceType",
            Label = "Type de de Résidence",
            Items =
            {
                new EnumItem{Code="RP",Label="Résidence Principale",Order=1},
                new EnumItem{Code="RS",Label="Résidence Secondaire",Order=2}
            }
        }, false);
        yield return AttributeDefinition.Create(new EnumDefinition
        {
            Key = "RealEstateUsage",
            Label = "Usage d’un bien immobilier",
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
                new EnumItem{Code="CON",Label = "Concession",Order=1},
                new EnumItem{Code= "STUDIO", Label="Appartement à une piece (studio)",Order=2}
            }
        }, false);
        yield return AttributeDefinition.Create(new EnumDefinition
        {
            Key = "RealEstateOwnerShip",
            Label = "Droit de Proprieté",
            Items =
            {
                new EnumItem {Code= "OWNER", Label="Propriétaire" },
                new EnumItem {Code="RENT", Label="Locataire"}
            }
        }, false);
        yield return AttributeDefinition.Create(new EnumDefinition
        {
            Key = "ResidenceStatus",
            Label = "Statut de la Residence",
            Items =
            {
                new EnumItem{Code="NEW",Label="Nouvelle", Order=1},
                new EnumItem{Code="OLD",Label="Ancienne", Order=2}
            }
        }, false);
        yield return AttributeDefinition.Create("AcquisitionDate", "Acquisition Date", AttributeDataType.Date);
        yield return AttributeDefinition.Create("BuildingCompletionDate", "Date de fin des Travaux", AttributeDataType.Date); ;


    }
    public static IEnumerable<AssetType> InitialData()
    {
        var realEstate = AssetType.Create("Real Estate", "Propiété Immobilière Maison et Terrain");
        foreach (var attr in RealEstateAttributes())
        {
            realEstate.AddExpectedAttribute(attr);
        }
        foreach (var rule in RealEstateTaxRules())
        {
            realEstate.AddTaxRule(rule);
        }
        yield return realEstate;

    }
}

