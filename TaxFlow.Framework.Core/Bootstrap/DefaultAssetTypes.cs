using System;
using Core.Domain.Contracts;
using Core.Domain.Enums;
using Core.Domain.Tax.Assets;
using Core.Domain.Tax.Calculation;
using Core.Domain.Tax.Obligations;
using Core.Domain.Tax.Penalties;

namespace Core.Bootstrap;

/// <summary>
/// Provides default asset types and their configurations.
/// </summary>
public static class DefaultAssetTypes
{
    /// <summary>
    /// Gets the initial data for asset types.
    /// </summary>
    public static IEnumerable<AssetType> InitialData()
    {
        yield return CreateRealEstateAssetType();
        yield return CreateTransportOperatorAssetType();
        yield return CreateEconomicActivityAssetType();
        yield return CreateLegalActAssetType();
        yield return CreatePersonalIncomeAssetType();
        yield return CreatePenaltyAssetType();
    }

    private static AssetType CreateRealEstateAssetType()
    {
        var realEstate = AssetType.Create("Real Estate", "Propriété Immobilière Maison et Terrain");

        foreach (var attr in RealEstateAttributes())
            realEstate.AddExpectedAttribute(attr);

        foreach (var rule in RealEstateTaxRules())
            realEstate.AddTaxRule(rule);

        return realEstate;
    }

    private static IEnumerable<AttributeDefinition> RealEstateAttributes()
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

    private static IEnumerable<TaxRule> RealEstateTaxRules()
    {
        yield return new TaxRule
        {
            Key = "TH",
            Label = "TAXE D'HABITATION",
            Description = "Barème forfaitaire selon la catégorie d'habitation.",
            ValidFrom = default,
            Expression = """
            [RealEstateCategory]=="Appartement 1 pièce (studio)"?2_000:
            [RealEstateCategory]=="Concession"?4_000:
            [RealEstateCategory]=="Appartement 2 pièces"?6_000:
            [RealEstateCategory]=="Appartement 3 pièces et plus"?9_000:
            [RealEstateCategory]=="Villa / concession unique"?30_000:
            [RealEstateCategory]=="Maison R+1"?40_000:
            [RealEstateCategory]=="Maison R+2"?75_000:
            [RealEstateCategory]=="Maison R+3 et plus"?100_000:
            [RealEstateCategory]=="Étage > 600 m²"?100_000:0
            """
        };

        yield return new TaxRule
        {
            Key = "TFPB",
            Label = "TAXE FONCIÈRE SUR PROPRIÉTÉS BÂTIES",
            Description = "Taux proportionnel annuel de 7,5 % sur la valeur locative.",
            ValidFrom = default,
            Expression = """
            [RealEstateType]=="Propriété Bâtie"?[LocativeValue]*7.5/100:0
            """
        };

        yield return new TaxRule
        {
            Key = "TFPNB",
            Label = "TAXE FONCIÈRE SUR PROPRIÉTÉS NON BÂTIES",
            Description = "Taux proportionnel annuel de 0,5 % sur la valeur vénale du terrain.",
            ValidFrom = default,
            Expression = """
            [RealEstateType]=="Propriété Non Bâtie"?[ResidualValue]*0.5/100:0
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

    private static AssetType CreateTransportOperatorAssetType()
    {
        var transport = AssetType.Create("Transport Operators", "Exploitants assujettis à la TPU-TR");

        foreach (var attr in TransportOperatorAttributes())
            transport.AddExpectedAttribute(attr);

        foreach (var rule in TransportOperatorTaxRules())
            transport.AddTaxRule(rule);

        return transport;
    }

    private static IEnumerable<AttributeDefinition> TransportOperatorAttributes()
    {
        yield return AttributeDefinition.Create(new EnumDefinition
        {
            Key = "TransportActivity",
            Label = "Type de transport routier",
            Items =
            {
                new EnumItem { Code = "SABLE", Label = "Transport de sable et gravats", Order = 1 },
                new EnumItem { Code = "MARCHAND", Label = "Transport de marchandises", Order = 2 },
                new EnumItem { Code = "PERSONNES", Label = "Transport de personnes", Order = 3 },
                new EnumItem { Code = "TAXIMOTO", Label = "Taximoto", Order = 4 },
                new EnumItem { Code = "TRICYCLE", Label = "Tricycle", Order = 5 },
                new EnumItem { Code = "PIROGUE", Label = "Pirogue", Order = 6 },
                new EnumItem { Code = "BATEAU", Label = "Bateau", Order = 7 }
            }
        });
        yield return AttributeDefinition.Create("VehicleTonnage", "Tonnage (en tonnes)", AttributeDataType.Number);
        yield return AttributeDefinition.Create("VehicleAgeYears", "Âge du véhicule (années)", AttributeDataType.Number);
        yield return AttributeDefinition.Create("SeatCount", "Nombre de sièges", AttributeDataType.Number);
        yield return AttributeDefinition.Create(new EnumDefinition
        {
            Key = "OperationZone",
            Label = "Zone d'exploitation",
            Items =
            {
                new EnumItem { Code = "GRANDE", Label = "Grande ville", Order = 1 },
                new EnumItem { Code = "VILLE", Label = "Ville", Order = 2 },
                new EnumItem { Code = "RURALE", Label = "Zone rurale", Order = 3 }
            }
        });
    }

    private static IEnumerable<TaxRule> TransportOperatorTaxRules()
    {
        var rule = new TaxRule
        {
            Key = "TPU_TR",
            Label = "TPU – Transporteurs routiers",
            Description = "Taxe forfaitaire quadrimestrielle basée sur tonnage, sièges, âge et zone.",
            Expression = """
            [TransportActivity]=="Transport de sable et gravats"?
                ([VehicleTonnage]<=10?9_000:[VehicleTonnage]<=20?11_000:13_500):
            [TransportActivity]=="Transport de marchandises"?
                (([VehicleTonnage]<=5?6_000:[VehicleTonnage]<=10?9_000:[VehicleTonnage]<=20?12_000:15_000)*
                 ([VehicleAgeYears]<=1?1.2:[VehicleAgeYears]<=3?1:[VehicleAgeYears]<=6?0.85:0.7)*
                 ([OperationZone]=="Grande ville"?1:[OperationZone]=="Ville"?0.9:0.75)):
            [TransportActivity]=="Transport de personnes"?
                (([SeatCount]<=5?6_000:[SeatCount]<=15?10_000:[SeatCount]<=30?15_000:20_000)*
                 ([VehicleAgeYears]<=2?1.1:[VehicleAgeYears]<=5?1:0.85)*
                 ([OperationZone]=="Grande ville"?1:[OperationZone]=="Ville"?0.95:0.75)):
            [TransportActivity]=="Taximoto"?
                ([OperationZone]=="Zone rurale"?2_500:4_000):
            [TransportActivity]=="Tricycle"?
                ([OperationZone]=="Zone rurale"?6_000:8_000):
            [TransportActivity]=="Pirogue"?
                ([OperationZone]=="Zone rurale"?3_000:5_000):
            [TransportActivity]=="Bateau"?
                ([OperationZone]=="Zone rurale"?5_000:9_000):
            0
            """
        };

        rule.ConfigureObligationSchedule(CreateQuadrimesterSchedule());
        yield return rule;
    }

    private static TaxObligationSchedule CreateQuadrimesterSchedule()
    {
        var year = DateTimeOffset.UtcNow.Year;
        var schedule = TaxObligationSchedule.Create("TPU-TR quadrimestriel", year);

        schedule.AddPaymentDeadline(PaymentDeadline.Create(
            "TPU_TR_Q1",
            "Quadrimestre 1",
            new DateTimeOffset(year, 4, 30, 0, 0, 0, TimeSpan.Zero),
            0.34m,
            1,
            Duration.Days(15)));

        schedule.AddPaymentDeadline(PaymentDeadline.Create(
            "TPU_TR_Q2",
            "Quadrimestre 2",
            new DateTimeOffset(year, 8, 31, 0, 0, 0, TimeSpan.Zero),
            0.33m,
            2,
            Duration.Days(15)));

        schedule.AddPaymentDeadline(PaymentDeadline.Create(
            "TPU_TR_Q3",
            "Quadrimestre 3",
            new DateTimeOffset(year, 12, 31, 0, 0, 0, TimeSpan.Zero),
            0.33m,
            3,
            Duration.Days(15)));

        return schedule;
    }

    private static AssetType CreateEconomicActivityAssetType()
    {
        var activity = AssetType.Create("Economic Activity", "Activités économiques soumises à la TPU");

        foreach (var attr in EconomicActivityAttributes())
            activity.AddExpectedAttribute(attr);

        foreach (var rule in EconomicActivityTaxRules())
            activity.AddTaxRule(rule);

        return activity;
    }

    private static IEnumerable<AttributeDefinition> EconomicActivityAttributes()
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
                new EnumItem { Code = "ELV", Label = "Éleveur", Order = 5 }
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

    private static IEnumerable<TaxRule> EconomicActivityTaxRules()
    {
        yield return new TaxRule
        {
            Key = "TPU_ECO",
            Label = "TPU – Activités économiques",
            Description = "Barème forfaitaire commerce/services et montants différenciés artisans/ambulants.",
            Expression = """
            [HasFranchise]==true?0:
            [ActivityNature]=="Commerce"?
                ([AnnualTurnover]<=2_500_000?10_000:
                 [AnnualTurnover]<=5_000_000?40_000:
                 [AnnualTurnover]<=10_000_000?115_000:
                 [AnnualTurnover]<=15_000_000?190_000:
                 [AnnualTurnover]<=20_000_000?265_000:
                 [AnnualTurnover]<=25_000_000?340_000:
                 [AnnualTurnover]<=30_000_000?415_000:500_000):
            [ActivityNature]=="Services"?
                (([AnnualTurnover]<=2_500_000?10_000:
                  [AnnualTurnover]<=5_000_000?40_000:
                  [AnnualTurnover]<=10_000_000?115_000:
                  [AnnualTurnover]<=15_000_000?190_000:
                  [AnnualTurnover]<=20_000_000?265_000:
                  [AnnualTurnover]<=25_000_000?340_000:
                  [AnnualTurnover]<=30_000_000?415_000:500_000)*1.35):
            [ActivityNature]=="Artisan"?
                (([UsesMechanicalMeans]==true?30_000:15_000)*
                 ([LocationCategory]=="Urbain"?1:[LocationCategory]=="Semi-urbain"?0.85:0.65)):
            [ActivityNature]=="Ambulant"?
                (10_000*([LocationCategory]=="Urbain"?1:[LocationCategory]=="Semi-urbain"?0.9:0.7)):
            [ActivityNature]=="Eleveur"?
                (8_000*([LocationCategory]=="Urbain"?1:[LocationCategory]=="Semi-urbain"?0.85:0.6)):
            0
            """
        };
    }

    private static AssetType CreateLegalActAssetType()
    {
        var legalAct = AssetType.Create("Legal Act", "Actes soumis aux droits d'enregistrement");

        foreach (var attr in LegalActAttributes())
            legalAct.AddExpectedAttribute(attr);

        foreach (var rule in LegalActTaxRules())
            legalAct.AddTaxRule(rule);

        return legalAct;
    }

    private static IEnumerable<AttributeDefinition> LegalActAttributes()
    {
        yield return AttributeDefinition.Create("ContractAmount", "Montant contractuel", AttributeDataType.Number, true);
    }

    private static IEnumerable<TaxRule> LegalActTaxRules()
    {
        yield return new TaxRule
        {
            Key = "ENR",
            Label = "DROITS D'ENREGISTREMENT",
            Description = "Taxe proportionnelle de 2 % (actuellement inactive).",
            Enabled = false,
            Expression = """
            [ContractAmount]*2/100
            """
        };
    }

    private static AssetType CreatePersonalIncomeAssetType()
    {
        var income = AssetType.Create("Household Income", "Impôts personnels IRF/IRTS/IRPRV/IRCM/IRGM", LiquidationMode.Grouped);

        foreach (var attr in PersonalIncomeAttributes())
            income.AddExpectedAttribute(attr);

        foreach (var rule in PersonalIncomeTaxRules())
            income.AddTaxRule(rule);

        return income;
    }

    private static IEnumerable<AttributeDefinition> PersonalIncomeAttributes()
    {
        yield return AttributeDefinition.Create("AnnualGlobalIncome", "Revenu global annuel", AttributeDataType.Number);
        yield return AttributeDefinition.Create("PensionAmount", "Pensions et rentes", AttributeDataType.Number);
        yield return AttributeDefinition.Create("CapitalIncomeAmount", "Revenus de capitaux mobiliers", AttributeDataType.Number);
        yield return AttributeDefinition.Create("ManagerRemuneration", "Rémunérations de gérance", AttributeDataType.Number);
    }

    private static IEnumerable<TaxRule> PersonalIncomeTaxRules()
    {
        yield return new TaxRule
        {
            Key = "IRPRV",
            Label = "IMPÔT SUR PENSIONS ET RENTES",
            Description = "Tranches 2,4-3,6M à 25 %, au-delà 50 %.",
            Expression = """
            [PensionAmount]<=2_400_000?0:
            [PensionAmount]<=3_600_000?([PensionAmount]-2_400_000)*0.25:
            ((3_600_000-2_400_000)*0.25)+([PensionAmount]-3_600_000)*0.50
            """
        };

        yield return new TaxRule
        {
            Key = "IRTS",
            Label = "IR SUR TRAITEMENTS ET SALAIRES",
            Description = "Barème IRPP sur le revenu global.",
            Expression = BuildIrppScaleExpression("AnnualGlobalIncome")
        };

        yield return new TaxRule
        {
            Key = "IRCM",
            Label = "IMPÔT SUR REVENUS DE CAPITAUX MOBILIERS",
            Description = "Taxe proportionnelle de 15 % sur les revenus agrégés.",
            Expression = """
            [CapitalIncomeAmount]*0.15
            """
        };

        yield return new TaxRule
        {
            Key = "IRGM",
            Label = "IMPÔT SUR REVENUS DE GÉRANTS",
            Description = "Taxe proportionnelle de 10 % sur les rémunérations des gérants/associés.",
            Expression = """
            [ManagerRemuneration]*0.10
            """
        };
    }

    private static AssetType CreatePenaltyAssetType()
    {
        var penalties = AssetType.Create("Recovery Penalties", "Pénalité de recouvrement (PENAR)");

        foreach (var attr in PenaltyAttributes())
            penalties.AddExpectedAttribute(attr);

        foreach (var rule in PenaltyTaxRules())
            penalties.AddTaxRule(rule);

        return penalties;
    }

    private static IEnumerable<AttributeDefinition> PenaltyAttributes()
    {
        yield return AttributeDefinition.Create("OutstandingTaxAmount", "Montant en souffrance", AttributeDataType.Number, true);
    }

    private static IEnumerable<TaxRule> PenaltyTaxRules()
    {
        yield return new TaxRule
        {
            Key = "PENAR",
            Label = "PÉNALITÉ DE RECOUVREMENT",
            Description = "Taux proportionnel de 10 % avec plancher 1 000.",
            Expression = """
            ([OutstandingTaxAmount]*0.10)<1_000?1_000:[OutstandingTaxAmount]*0.10
            """
        };
    }

    private static string BuildIrppScaleExpression(string variableName) =>
        $"""
        (
            (([{variableName}]>900_000?([{variableName}]<3_000_000?[{variableName}]:3_000_000):900_000)-900_000)*0.10 +
            (([{variableName}]>3_000_000?([{variableName}]<9_000_000?[{variableName}]:9_000_000):3_000_000)-3_000_000)*0.15 +
            (([{variableName}]>9_000_000?([{variableName}]<12_000_000?[{variableName}]:12_000_000):9_000_000)-9_000_000)*0.20 +
            (([{variableName}]>12_000_000?([{variableName}]<15_000_000?[{variableName}]:15_000_000):12_000_000)-12_000_000)*0.25 +
            (([{variableName}]>15_000_000?([{variableName}]<20_000_000?[{variableName}]:20_000_000):15_000_000)-15_000_000)*0.30 +
            (([{variableName}]>20_000_000?[{variableName}]:20_000_000)-20_000_000)*0.35
        )
        """;
}
