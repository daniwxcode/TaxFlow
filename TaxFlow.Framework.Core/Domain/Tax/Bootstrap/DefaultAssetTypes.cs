using Core.Domain.Contracts;
using Core.Domain.Enums;

namespace Core.Domain.Tax.Bootstrap;

public static class DefaultAssetTypes
{
    public static IEnumerable<AssetType> InitialData()
    {
        var realEstate = AssetType.Create("Real Estate", "Propiété Immobilière Maison et Terrain");

        var realEstateType = new EnumDefinition
        {
            Key = "RealEstateType",
            Label = "Type de Propriété",
            Items =
            {
                new EnumItem{Code="PB",Label="Propriété Bâtie",Order=1},
                new EnumItem{Code="PB",Label="Propriété Non Bâtie", Order=2}
            }

        };
        var realEstateUsage = new EnumDefinition
        {
            Key = "RealEstateUsage",
            Label = "Usage d’un bien immobilier",
            Items =
                    {
                        new EnumItem { Code = "RES", Label = "Résidentiel", Order = 1 },
                        new EnumItem { Code = "COM", Label = "Location", Order = 2 }
                    }
        };
        var realEstateOwnerShip = new EnumDefinition
        {
            Key = "RealEstateOwnerShip",
            Label = "Droit de Proprieté",
            Items =
            {
                new EnumItem {Code= "OWNER", Label="Propriétaire" },
                new EnumItem {Code="RENT", Label="Locataire"}
            }
        };
        var realEstateCategory = new EnumDefinition
        {
            Key = "RealEstateCategory",
            Label = "Type d'Habitat",
            Items =
            {
                new EnumItem{Code="CON",Label = "Concession",Order=1},
                new EnumItem{Code= "STUDIO", Label="Appartement à une piece (studio)",Order=2}
            }
        };
        var residenceType = new EnumDefinition
        {
            Key = "ResidenceType",
            Label = "Type de de Résidence",
            Items =
            {
                new EnumItem{Code="RP",Label="Résidence Principale",Order=1},
                new EnumItem{Code="RS",Label="Résidence Secondaire",Order=2}
            }
        };
        var residenceStatus = new EnumDefinition
        {
            Key = "ResidenceStatus",
            Label = "Statut de la Residence",
            Items =
            {
                new EnumItem{Code="NEW",Label="Nouvelle", Order=1},
                new EnumItem{Code="OLD",Label="Ancienne", Order=2}
            }
        };


        // Gestion des Taxes
        var tfnbRule = new TaxRule()
        {
            Key = "TFNB",
            Label = "TAXE FONCIERE SUR PROPRIETE NON BATIE",
            ValidFrom = default,
            Expression = """
            ([RealEstateType]=="Propriété Bâtie"||[RealEstateUsage]=="Location")?0:[ResidualValue]*0.5/100
            """
        };
        var tfbRule = new TaxRule()
        {
            Key = "TFNB",
            Label = "TAXE FONCIERE SUR PROPRIETE BATIE",
            ValidFrom = default,
            Expression = """
            ([RealEstateType]=="Propriété Bâtie"||[RealEstateUsage]=="Location")?[ResidualValue]*0.75/100:0
            """
        };
        //

        realEstate.AddTaxRule(tfnbRule).AddTaxRule(tfbRule);

        yield return realEstate

       .AddExpectedAttribute(AttributeDefinition.Create("ResidualValue", "Valeur Venale", AttributeDataType.Number,true))
       .AddExpectedAttribute(AttributeDefinition.Create("Situation", "Situation", AttributeDataType.String))
       .AddExpectedAttribute(AttributeDefinition.Create(realEstateType))
       .AddExpectedAttribute(AttributeDefinition.Create(residenceType, false))
       .AddExpectedAttribute(AttributeDefinition.Create(realEstateUsage))
       .AddExpectedAttribute(AttributeDefinition.Create(realEstateCategory))
       .AddExpectedAttribute(AttributeDefinition.Create(realEstateOwnerShip))
       .AddExpectedAttribute(AttributeDefinition.Create(residenceStatus))
       .AddExpectedAttribute(AttributeDefinition.Create("AcquisitionDate", "Acquisition Date", AttributeDataType.Date))
       .AddExpectedAttribute(AttributeDefinition.Create("BuildingCompletionDate", "Date de fin des Travaux", AttributeDataType.Date));


    }
}

