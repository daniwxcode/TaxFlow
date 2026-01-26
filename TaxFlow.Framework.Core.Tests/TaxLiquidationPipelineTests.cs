using Core.Domain.Contracts;
using Core.Domain.Enums;
using Core.Domain.Tax.Assets;
using Core.Domain.Tax.Calculation;
using Core.Domain.Tax.Calculation.Services;

using System.Collections.ObjectModel;
using System.Linq;

using Xunit;

namespace TaxFlow.Framework.Core.Tests;

public class TaxLiquidationPipelineTests
{
    [Fact]
    public void Evaluate_GroupedAssets_AggregatesNumericAttributes()
    {
        AssetType assetType = AssetType.Create("Grouped Income", null, LiquidationMode.Grouped);
        assetType.AddExpectedAttribute(AttributeDefinition.Create("AnnualGlobalIncome", "Revenu", AttributeDataType.Number, true));
        assetType.AddTaxRule(new TaxRule
        {
            Key = "IRTS",
            Label = "IRTS",
            Expression = "[AnnualGlobalIncome]>500000?[AnnualGlobalIncome]*0.2:[AnnualGlobalIncome]*0.1"
        });

        TaxableAsset asset1 = TaxableAsset.Create(assetType, new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("AnnualGlobalIncome", "300000", AttributeDataType.Number, true)
        });

        TaxableAsset asset2 = TaxableAsset.Create(assetType, new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("AnnualGlobalIncome", "300000", AttributeDataType.Number, true)
        });

        TaxCalculationResult result = TaxLiquidationPipeline.Evaluate(new[] { asset1, asset2 }, new TaxEngineOptions { IncludeRuleResults = false });

        Assert.Single(result.Lines);
        Assert.Equal(120_000m, result.Lines.First().Amount);
    }

    [Fact]
    public void Evaluate_IndividualAssets_KeepsLinesSeparated()
    {
        AssetType assetType = AssetType.Create("Solo", null, LiquidationMode.Individual);
        assetType.AddExpectedAttribute(AttributeDefinition.Create("Base", "Base", AttributeDataType.Number, true));
        assetType.AddTaxRule(new TaxRule
        {
            Key = "R1",
            Label = "Rule",
            Expression = "[Base]*0.1"
        });

        TaxableAsset first = TaxableAsset.Create(assetType, new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("Base", "100", AttributeDataType.Number, true)
        });

        TaxableAsset second = TaxableAsset.Create(assetType, new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("Base", "200", AttributeDataType.Number, true)
        });

        TaxCalculationResult result = TaxLiquidationPipeline.Evaluate(new[] { first, second }, new TaxEngineOptions { IncludeRuleResults = false });

        Assert.Equal(2, result.Lines.Count);
        Assert.Equal(10m, result.Lines.First().Amount);
        Assert.Equal(20m, result.Lines.Last().Amount);
    }
}
