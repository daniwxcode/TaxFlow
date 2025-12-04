using System.Linq;
using System.Collections.ObjectModel;
using Core.Domain.Tax.Bootstrap;
using Core.Domain.Contracts;
using Core.Domain.Enums;
using Xunit;

namespace TaxFlow.Framework.Core.Tests;

public class DefaultAssetTypesTests
{
    [Fact]
    public void InitialData_ContainsRealEstate_WithExpectedAttributesAndRules()
    {
        var list = DefaultAssetTypes.InitialData().ToList();
        Assert.NotEmpty(list);

        var realEstate = list.FirstOrDefault(a => a.Name == "Real Estate");
        Assert.NotNull(realEstate);

        // Expected attribute keys
        var expectedKeys = new[] {
            "ResidualValue",
            "Situation",
            "RealEstateType",
            "ResidenceType",
            "RealEstateUsage",
            "RealEstateCategory",
            "RealEstateOwnerShip",
            "ResidenceStatus",
            "AcquisitionDate",
            "BuildingCompletionDate"
        };

        foreach (var key in expectedKeys)
        {
            Assert.True(realEstate!.ExpectedAttributes.Any(e => e.Key == key), $"Expected attribute '{key}' not found.");
        }

        // Tax rules
        Assert.NotEmpty(realEstate.TaxRules);
        Assert.Equal(2, realEstate.TaxRules.Count);

        var labels = realEstate.TaxRules.Select(r => r.Label).ToList();
        Assert.Contains(labels, l => l.Contains("TAXE FONCIERE"));

        // Expressions contain expected multipliers
        var exprs = realEstate.TaxRules.Select(r => r.Expression).ToList();
        Assert.Contains(exprs, e => e.Contains("*0.5/100") || e.Contains("*0.5/100\r") );
        Assert.Contains(exprs, e => e.Contains("*0.75/100") || e.Contains("*0.75/100\r"));
    }

    [Fact]
    public void TaxRule_Expressions_CanBeEvaluatedWithSampleAttributes()
    {
        var list = DefaultAssetTypes.InitialData().ToList();
        var realEstate = list.First(a => a.Name == "Real Estate");

        // Prepare attributes: ResidualValue and RealEstateType = "Propriété Bâtie"
        var attrs = new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("ResidualValue", "1000000", AttributeDataType.Number, true),
            ExtendedAttribute.Create("RealEstateType", "Propriété Bâtie", AttributeDataType.Enum, true),
        };

        // Evaluate each rule by its key (use the key from the rule instance)
        foreach (var rule in realEstate.TaxRules)
        {
            var value = realEstate.EvaluateTaxRule(rule.Key, attrs);
            // Evaluation should at least return a decimal or zero (not throw)
            Assert.True(value == null || value is decimal);
        }

        // Additionally ensure that at least one rule produces a positive amount for this configuration
        var anyPositive = realEstate.TaxRules.Any(r => (realEstate.EvaluateTaxRule(r.Key, attrs) ?? 0m) > 0m);
        Assert.True(anyPositive, "At least one tax rule should produce a positive amount for the provided attributes.");
    }
}
