using Core.Domain.Contracts;
using Core.Domain.Enums;
using Core.Domain.Tax.Assets;
using Core.Domain.Tax.Calculation;
using Core.Domain.Tax.Obligations;
using Core.Domain.Tax.Penalties;

namespace Core.Bootstrap.AssetTypes;

/// <summary>
/// Asset type definition for Transport Operators (Exploitants TPU-TR).
/// </summary>
public sealed class TransportOperatorAssetTypeDefinition : IAssetTypeDefinition
{
    /// <summary>
    /// Gets the unique identifier for this asset type.
    /// </summary>
    public string AssetTypeKey => "TRANSPORT_OPERATOR";
    /// <summary>
    /// Gets the human-readable name of the asset type.
    /// </summary>
    public string Name => "Transport Operators";
    /// <summary>
    /// Gets the description of the asset type.
    /// </summary>
    public string Description => "Exploitants assujettis à la TPU-TR";
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
        var assetType = AssetType.Create(Name, Description, LiquidationMode);

        foreach (var attr in GetAttributes())
            assetType.AddExpectedAttribute(attr);

        foreach (var rule in GetTaxRules())
            assetType.AddTaxRule(rule);

        return assetType;
    }

    private IEnumerable<AttributeDefinition> GetAttributes()
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

    private IEnumerable<TaxRule> GetTaxRules()
    {
        var rule = new TaxRule
        {
            Key = "TPU_TR",
            Label = "TPU – Transporteurs routiers",
            Description = "Taxe forfaitaire trrimestrielle basée sur tonnage, sièges, âge et zone.",
            Expression = """
            ([TransportActivityCode]=="SABLE"?
                ([VehicleTonnage]<=10?9000:([VehicleTonnage]<=20?11000:13500))
            :0) +
            ([TransportActivityCode]=="MARCHAND"?
                (
                    ([VehicleTonnage]<=5?6000:([VehicleTonnage]<=10?9000:([VehicleTonnage]<=20?12000:15000))) *
                    ([VehicleAgeYears]<=1?1.2:([VehicleAgeYears]<=3?1:([VehicleAgeYears]<=6?0.85:0.7))) *
                    ([OperationZoneCode]=="GRANDE"?1:([OperationZoneCode]=="VILLE"?0.9:0.75))
                )
            :0) +
            ([TransportActivityCode]=="PERSONNES"?
                (
                    ([SeatCount]<=5?6000:([SeatCount]<=15?10000:([SeatCount]<=30?15000:20000))) *
                    ([VehicleAgeYears]<=2?1.1:([VehicleAgeYears]<=5?1:0.85)) *
                    ([OperationZoneCode]=="GRANDE"?1:([OperationZoneCode]=="VILLE"?0.95:0.75))
                )
            :0) +
            ([TransportActivityCode]=="TAXIMOTO"?
                ([OperationZoneCode]=="RURALE"?2500:4000)
            :0) +
            ([TransportActivityCode]=="TRICYCLE"?
                ([OperationZoneCode]=="RURALE"?6000:8000)
            :0) +
            ([TransportActivityCode]=="PIROGUE"?
                ([OperationZoneCode]=="RURALE"?3000:5000)
            :0) +
            ([TransportActivityCode]=="BATEAU"?
                ([OperationZoneCode]=="RURALE"?5000:9000)
            :0)
            """
        };

        rule.ConfigureObligationSchedule(CreateQuarterlySchedule());
        yield return rule;
    }

    private static TaxObligationSchedule CreateQuarterlySchedule()
    {
        var year = DateTimeOffset.UtcNow.Year;
        var schedule = TaxObligationSchedule.Create("TPU-TR trimestriel", year);

        schedule.AddPaymentDeadline(PaymentDeadline.Create(
            "TPU_TR_T1", "Trimestre 1",
            new DateTimeOffset(year, 3, 31, 0, 0, 0, TimeSpan.Zero), 0.25m, 1, Duration.Months(1)));

        schedule.AddPaymentDeadline(PaymentDeadline.Create(
            "TPU_TR_T2", "Trimestre 2",
            new DateTimeOffset(year, 6, 30, 0, 0, 0, TimeSpan.Zero), 0.25m, 2, Duration.Months(1)));

        schedule.AddPaymentDeadline(PaymentDeadline.Create(
            "TPU_TR_T3", "Trimestre 3",
            new DateTimeOffset(year, 9, 30, 0, 0, 0, TimeSpan.Zero), 0.25m, 3, Duration.Months(1)));

        schedule.AddPaymentDeadline(PaymentDeadline.Create(
            "TPU_TR_T4", "Trimestre 4",
            new DateTimeOffset(year, 12, 31, 0, 0, 0, TimeSpan.Zero), 0.25m, 4, Duration.Months(1)));

        return schedule;
    }
}
