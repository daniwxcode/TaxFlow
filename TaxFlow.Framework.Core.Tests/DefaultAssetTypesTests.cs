using System.Collections.ObjectModel;

using Core.Bootstrap;
using Core.Domain.Contracts;
using Core.Domain.Enums;
using Core.Domain.Tax.Assets;
using Core.Domain.Tax.Calculation;

using Xunit;

namespace TaxFlow.Framework.Core.Tests;
/// <summary>
/// Teste la classe DefaultAssetTypes.
/// </summary>
public class DefaultAssetTypesTests
{
    /// <summary>
    /// Verifie que la méthode InitialData expose tous les types d'actifs par défaut attendus.
    /// </summary>
    [Fact]
    public void InitialData_Exposes_All_Default_AssetTypes()
    {        
        List<AssetType> assetTypes = [.. DefaultAssetTypes.InitialData()];
        string[] expected = new[] {
            "Real Estate",
            "Transport Operators",
            "Economic Activity",
            "Legal Act",
            "Household Income",
            "Recovery Penalties"
        };

        foreach (string name in expected)
        {
            Assert.Contains(assetTypes, a => a.Name == name);
        }

        AssetType realEstate = assetTypes.First(a => a.Name == "Real Estate");
        string[] requiredAttributes = new[] {
            "ResidualValue",
            "LocativeValue",
            "NetRentalIncome",
            "AnnualRent",
            "RealEstateCategory"
        };

        foreach (string key in requiredAttributes)
        {
            Assert.Contains(realEstate.ExpectedAttributes, attr => attr.Key == key);
        }

        List<string> realEstateRuleKeys = realEstate.TaxRules.Select(r => r.Key).ToList();
        Assert.Contains("TH", realEstateRuleKeys);
        Assert.Contains("TFPB", realEstateRuleKeys);
        Assert.Contains("TFPNB", realEstateRuleKeys);
        Assert.Contains("IRF", realEstateRuleKeys);
        Assert.Contains("RSL", realEstateRuleKeys);
    }
    /// <summary>
    /// Verifie que les règles fiscales pour les biens immobiliers suivent les grilles spécifiées.
    /// </summary>
    [Fact]
    public void RealEstateRules_Follow_Specified_Grids()
    {
        AssetType realEstate = DefaultAssetTypes.InitialData().First(a => a.Name == "Real Estate");

        Collection<ExtendedAttribute> thAttrs = new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("RealEstateCategory", "APT2", AttributeDataType.Enum, true)
        };
        TaxRuleEvaluationResult thResult = realEstate.EvaluateTaxRuleDetailed("TH", thAttrs);
        Assert.True(thResult.IsSuccess, thResult.ErrorMessage ?? string.Empty);
        Assert.True(thResult.Value.HasValue, $"Warnings: {string.Join(", ", thResult.Warnings)}");
        Assert.Equal(6_000m, thResult.Value.Value);

        Collection<ExtendedAttribute> tfpbAttrs = new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("LocativeValue", "1200000", AttributeDataType.Number, true),
            ExtendedAttribute.Create("RealEstateType", "PB", AttributeDataType.Enum, true)
        };
        TaxRuleEvaluationResult tfpbResult = realEstate.EvaluateTaxRuleDetailed("TFPB", tfpbAttrs);
        Assert.True(tfpbResult.IsSuccess, tfpbResult.ErrorMessage ?? string.Empty);
        Assert.True(tfpbResult.Value.HasValue, $"Warnings: {string.Join(", ", tfpbResult.Warnings)}");
        Assert.Equal(90_000m, tfpbResult.Value.Value);

        Collection<ExtendedAttribute> tfpnbAttrs = new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("ResidualValue", "800000", AttributeDataType.Number, true),
            ExtendedAttribute.Create("RealEstateType", "PNB", AttributeDataType.Enum, true)
        };
        TaxRuleEvaluationResult tfpnbResult = realEstate.EvaluateTaxRuleDetailed("TFPNB", tfpnbAttrs);
        Assert.True(tfpnbResult.IsSuccess, tfpnbResult.ErrorMessage ?? string.Empty);
        Assert.True(tfpnbResult.Value.HasValue, $"Warnings: {string.Join(", ", tfpnbResult.Warnings)}");
        Assert.Equal(4_000m, tfpnbResult.Value.Value);

        Collection<ExtendedAttribute> irfAttrs = new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("NetRentalIncome", "12500000", AttributeDataType.Number, true)
        };
        TaxRuleEvaluationResult irfResult = realEstate.EvaluateTaxRuleDetailed("IRF", irfAttrs);
        Assert.True(irfResult.IsSuccess, irfResult.ErrorMessage ?? string.Empty);
        Assert.True(irfResult.Value.HasValue, $"Warnings: {string.Join(", ", irfResult.Warnings)}");
        Assert.Equal(1_835_000m, irfResult.Value.Value);
    }
    /// <summary>
    /// Vérifie que les règles fiscales pour les transporteurs calculent les forfaits selon l'activité.
    /// </summary>
    [Fact]
    public void TransportRule_Calculates_Forfeits_By_Activity()
    {
        AssetType transport = DefaultAssetTypes.InitialData().First(a => a.Name == "Transport Operators");

        Collection<ExtendedAttribute> sandAttrs =
        [
            ExtendedAttribute.Create("TransportActivity", "SABLE", AttributeDataType.Enum, true),
            ExtendedAttribute.Create("VehicleTonnage", "18", AttributeDataType.Number, true)
        ];
        decimal? tax = transport.EvaluateTaxRule("TPU_TR", sandAttrs);
        Assert.Equal(11_000m, tax!.Value);

        var motoAttrs = new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("TransportActivity", "TAXIMOTO", AttributeDataType.Enum, true),
            ExtendedAttribute.Create("OperationZone", "RURALE", AttributeDataType.Enum, true)
        };
        Assert.Equal(2_500m, transport.EvaluateTaxRule("TPU_TR", motoAttrs));
    }
    /// <summary>
    /// Vérifie que les règles fiscales pour les activités économiques appliquent les barèmes commerciaux et de services.
    /// </summary>
    [Fact]
    public void EconomicActivityRule_Applies_Commercial_And_Service_Baremes()
    {
        var economic = DefaultAssetTypes.InitialData().First(a => a.Name == "Economic Activity");

        var commerceAttrs = new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("AnnualTurnover", "6000000", AttributeDataType.Number, true),
            ExtendedAttribute.Create("ActivityNature", "COM", AttributeDataType.Enum, true)
        };
        Assert.Equal(115_000m, economic.EvaluateTaxRule("TPU_ECO", commerceAttrs));

        var serviceAttrs = new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("AnnualTurnover", "6000000", AttributeDataType.Number, true),
            ExtendedAttribute.Create("ActivityNature", "SRV", AttributeDataType.Enum, true)
        };
        Assert.Equal(155_250m, economic.EvaluateTaxRule("TPU_ECO", serviceAttrs));
    }
    /// <summary>
    /// Vérifie que les règles fiscales pour les revenus et pénalités respectent les barèmes spécifiés.
    /// </summary>
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
    /// <summary>
    /// Vérifie que les types d'actifs configurent correctement les modes de liquidation.
    /// </summary>
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
    /// <summary>
    /// Vérifie que les expressions ternaires sont prises en charge par l'évaluateur.
    /// </summary>
    [Fact]
    public void Ternary_Expressions_Are_Supported_By_Evaluator()
    {
        var assetType = AssetType.Create("Test Asset");
        assetType.AddExpectedAttribute(AttributeDefinition.Create("Value", "Value", AttributeDataType.Number, true));
        assetType.AddTaxRule(new TaxRule { Key = "R1", Label = "Rule", Expression = "[Value]>0?1:2" });

        var attrs = new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("Value", "5", AttributeDataType.Number, true)
        };

        var result = assetType.EvaluateTaxRule("R1", attrs);
        Assert.Equal(1m, result);
    }
    /// <summary>
    /// Vérifie que les expressions ternaires peuvent s'étendre sur plusieurs lignes.
    /// </summary>
    [Fact]
    public void Ternary_Expressions_Can_Span_Multiple_Lines()
    {
        var assetType = AssetType.Create("Test Multi");
        assetType.AddExpectedAttribute(AttributeDefinition.Create("Code", "Code", AttributeDataType.String, true));
        assetType.AddTaxRule(new TaxRule
        {
            Key = "Multi",
            Label = "Multi",
            Expression = """
            ([Code]=="A"?1:0) +
            ([Code]=="B"?2:0)
            """
        });

        var attrs = new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("Code", "B", AttributeDataType.String, true)
        };

        var result = assetType.EvaluateTaxRule("Multi", attrs);
        Assert.Equal(2m, result);
    }
}
