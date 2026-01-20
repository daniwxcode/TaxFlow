using System.Collections.ObjectModel;
using System.Linq;
using Core.Bootstrap;
using Core.Domain.Contracts;
using Core.Domain.Enums;
using Xunit;

namespace TaxFlow.Framework.Core.Tests;

public class DefaultAssetTypesTests
{
    [Fact]
    public void InitialData_Exposes_All_Default_AssetTypes()
    {
        var assetTypes = DefaultAssetTypes.InitialData().ToList();
        var expected = new[] {
            "Real Estate",
            "Transport Operators",
            "Economic Activity",
            "Legal Act",
            "Household Income",
            "Recovery Penalties"
        };

        foreach (var name in expected)
            Assert.Contains(assetTypes, a => a.Name == name);

        var realEstate = assetTypes.First(a => a.Name == "Real Estate");
        var requiredAttributes = new[] {
            "ResidualValue",
            "LocativeValue",
            "NetRentalIncome",
            "AnnualRent",
            "RealEstateCategory"
        };

        foreach (var key in requiredAttributes)
            Assert.Contains(realEstate.ExpectedAttributes, attr => attr.Key == key);

        var realEstateRuleKeys = realEstate.TaxRules.Select(r => r.Key).ToList();
        Assert.Contains("TH", realEstateRuleKeys);
        Assert.Contains("TFPB", realEstateRuleKeys);
        Assert.Contains("TFPNB", realEstateRuleKeys);
        Assert.Contains("IRF", realEstateRuleKeys);
        Assert.Contains("RSL", realEstateRuleKeys);
    }

    [Fact]
    public void RealEstateRules_Follow_Specified_Grids()
    {
        var realEstate = DefaultAssetTypes.InitialData().First(a => a.Name == "Real Estate");

        var thAttrs = new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("RealEstateCategory", "Appartement 2 pièces", AttributeDataType.Enum, true)
        };
        Assert.Equal(6_000m, realEstate.EvaluateTaxRule("TH", thAttrs));

        var tfpbAttrs = new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("LocativeValue", "1200000", AttributeDataType.Number, true),
            ExtendedAttribute.Create("RealEstateType", "Propriété Bâtie", AttributeDataType.Enum, true)
        };
        Assert.Equal(90_000m, realEstate.EvaluateTaxRule("TFPB", tfpbAttrs));

        var tfpnbAttrs = new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("ResidualValue", "800000", AttributeDataType.Number, true),
            ExtendedAttribute.Create("RealEstateType", "Propriété Non Bâtie", AttributeDataType.Enum, true)
        };
        Assert.Equal(4_000m, realEstate.EvaluateTaxRule("TFPNB", tfpnbAttrs));

        var irfAttrs = new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("NetRentalIncome", "12500000", AttributeDataType.Number, true)
        };
        Assert.Equal(1_835_000m, realEstate.EvaluateTaxRule("IRF", irfAttrs));
    }

    [Fact]
    public void TransportRule_Calculates_Forfeits_By_Activity()
    {
        var transport = DefaultAssetTypes.InitialData().First(a => a.Name == "Transport Operators");

        var sandAttrs = new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("TransportActivity", "Transport de sable et gravats", AttributeDataType.Enum, true),
            ExtendedAttribute.Create("VehicleTonnage", "18", AttributeDataType.Number, true)
        };
        Assert.Equal(11_000m, transport.EvaluateTaxRule("TPU_TR", sandAttrs));

        var motoAttrs = new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("TransportActivity", "Taximoto", AttributeDataType.Enum, true),
            ExtendedAttribute.Create("OperationZone", "Zone rurale", AttributeDataType.Enum, true)
        };
        Assert.Equal(2_500m, transport.EvaluateTaxRule("TPU_TR", motoAttrs));
    }

    [Fact]
    public void EconomicActivityRule_Applies_Commercial_And_Service_Baremes()
    {
        var economic = DefaultAssetTypes.InitialData().First(a => a.Name == "Economic Activity");

        var commerceAttrs = new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("AnnualTurnover", "6000000", AttributeDataType.Number, true),
            ExtendedAttribute.Create("ActivityNature", "Commerce", AttributeDataType.Enum, true)
        };
        Assert.Equal(115_000m, economic.EvaluateTaxRule("TPU_ECO", commerceAttrs));

        var serviceAttrs = new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("AnnualTurnover", "6000000", AttributeDataType.Number, true),
            ExtendedAttribute.Create("ActivityNature", "Services", AttributeDataType.Enum, true)
        };
        Assert.Equal(155_250m, economic.EvaluateTaxRule("TPU_ECO", serviceAttrs));
    }

    [Fact]
    public void Income_And_Penalty_Rules_Respect_Specified_Scales()
    {
        var income = DefaultAssetTypes.InitialData().First(a => a.Name == "Household Income");

        var pensionAttrs = new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("PensionAmount", "3200000", AttributeDataType.Number, true)
        };
        Assert.Equal(200_000m, income.EvaluateTaxRule("IRPRV", pensionAttrs));

        var salaryAttrs = new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("AnnualGlobalIncome", "25000000", AttributeDataType.Number, true)
        };
        Assert.Equal(5_710_000m, income.EvaluateTaxRule("IRTS", salaryAttrs));

        var capitalAttrs = new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("CapitalIncomeAmount", "1000000", AttributeDataType.Number, true)
        };
        Assert.Equal(150_000m, income.EvaluateTaxRule("IRCM", capitalAttrs));

        var managerAttrs = new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("ManagerRemuneration", "2000000", AttributeDataType.Number, true)
        };
        Assert.Equal(200_000m, income.EvaluateTaxRule("IRGM", managerAttrs));

        var penalties = DefaultAssetTypes.InitialData().First(a => a.Name == "Recovery Penalties");
        var penaltyAttrs = new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("OutstandingTaxAmount", "5000", AttributeDataType.Number, true)
        };
        Assert.Equal(1_000m, penalties.EvaluateTaxRule("PENAR", penaltyAttrs));
    }

    [Fact]
    public void AssetTypes_Configure_Liquidation_Modes()
    {
        var assetTypes = DefaultAssetTypes.InitialData().ToList();

        var grouped = assetTypes.First(a => a.Name == "Household Income");
        Assert.Equal(LiquidationMode.Grouped, grouped.LiquidationMode);

        var soloNames = new[]
        {
            "Real Estate",
            "Transport Operators",
            "Economic Activity",
            "Legal Act",
            "Recovery Penalties"
        };

        foreach (var name in soloNames)
        {
            var asset = assetTypes.First(a => a.Name == name);
            Assert.Equal(LiquidationMode.Individual, asset.LiquidationMode);
        }
    }
}
