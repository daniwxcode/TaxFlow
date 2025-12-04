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

    [Fact]
    public void TaxRule_Evaluation_Produces_Expected_Amounts_For_Scenarios()
    {
        var realEstate = DefaultAssetTypes.InitialData().First(a => a.Name == "Real Estate");

        // Scenario 1: Propriété Bâtie => TFNB = 0, TFB = ResidualValue * 0.75/100
        var attrs1 = new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("ResidualValue", "1000000", AttributeDataType.Number, true),
            ExtendedAttribute.Create("RealEstateType", "Propriété Bâtie", AttributeDataType.Enum, true),
        };
        var tfnb1 = realEstate.EvaluateTaxRule("TFNB", attrs1);
        var tfb1 = realEstate.EvaluateTaxRule("TFB", attrs1);
        Assert.Equal(0m, tfnb1 ?? 0m);
        Assert.Equal(7500m, tfb1 ?? 0m);

        // Scenario 2: Usage = Location => TFNB = 0, TFB = ResidualValue * 0.75/100
        var attrs2 = new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("ResidualValue", "200000", AttributeDataType.Number, true),
            ExtendedAttribute.Create("RealEstateUsage", "Location", AttributeDataType.Enum, true),
        };
        var tfnb2 = realEstate.EvaluateTaxRule("TFNB", attrs2);
        var tfb2 = realEstate.EvaluateTaxRule("TFB", attrs2);
        Assert.Equal(0m, tfnb2 ?? 0m);
        Assert.Equal(1500m, tfb2 ?? 0m);

        // Scenario 3: Non bâtie and not Location => TFNB = ResidualValue * 0.5/100, TFB = 0
        var attrs3 = new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("ResidualValue", "500000", AttributeDataType.Number, true),
            ExtendedAttribute.Create("RealEstateType", "Propriété Non Bâtie", AttributeDataType.Enum, true),
        };
        var tfnb3 = realEstate.EvaluateTaxRule("TFNB", attrs3);
        var tfb3 = realEstate.EvaluateTaxRule("TFB", attrs3);
        Assert.Equal(2500m, tfnb3 ?? 0m);
        Assert.Equal(0m, tfb3 ?? 0m);
    }

    [Fact]
    public void ValidateAttributes_Detects_MissingRequired_And_Invalid_Types_And_Enum_Values()
    {
        var realEstate = DefaultAssetTypes.InitialData().First(a => a.Name == "Real Estate");

        // Missing required ResidualValue
        var missing = new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("RealEstateType", "PB", AttributeDataType.Enum, true),
        };
        var errorsMissing = realEstate.ValidateAttributes(missing).ToList();
        Assert.Contains(errorsMissing, e => e.Contains("ResidualValue") || e.Contains("requis"));

        // Wrong data type for ResidualValue (enum provided instead of number)
        var wrongType = new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("ResidualValue", "PB", AttributeDataType.Enum, true),
        };
        var errorsType = realEstate.ValidateAttributes(wrongType).ToList();
        Assert.Contains(errorsType, e => e.Contains("Type invalide") || e.Contains("Type invalide"));

        // Enum provided with label instead of code should fail validation
        var invalidEnumValue = new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("ResidualValue", "100000", AttributeDataType.Number, true),
            // using label instead of code - validation expects codes like "PB" or "PNB"
            ExtendedAttribute.Create("RealEstateType", "Propriété Bâtie", AttributeDataType.Enum, true),
        };
        var errorsEnum = realEstate.ValidateAttributes(invalidEnumValue).ToList();
        Assert.Contains(errorsEnum, e => e.Contains("n'est pas dans les valeurs autorisées") || e.Contains("Valeur invalide"));

        // Correct attributes should produce no errors (use enum codes)
        var valid = new Collection<ExtendedAttribute>
        {
            ExtendedAttribute.Create("ResidualValue", "100000", AttributeDataType.Number, true),
            ExtendedAttribute.Create("RealEstateType", "PB", AttributeDataType.Enum, true),
            ExtendedAttribute.Create("RealEstateUsage", "RES", AttributeDataType.Enum, false),
        };
        var errorsValid = realEstate.ValidateAttributes(valid).ToList();
        Assert.Empty(errorsValid);
    }
}
