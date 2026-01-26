using Core.Domain.Contracts;
using Core.Domain.Enums;
using Core.Domain.Tax.Assets;
using Core.Domain.Tax.Calculation;

using System.Collections.ObjectModel;
using System.Linq;

using Xunit;

namespace TaxFlow.Framework.Core.Tests;

/// <summary>
/// Contient des tests unitaires pour les types d'actifs et les actifs imposables.
/// </summary>
public class AssetTypeAndTaxableAssetTests
{
    /// <summary>
    /// Vérifie l'ajout et la suppression d'un attribut attendu sur un type d'actif.
    /// </summary>
    [Fact]
    public void AssetType_AddAndRemoveExpectedAttribute()
    {
        var at = AssetType.Create("A");
        var def = AttributeDefinition.Create("K", "L", AttributeDataType.String);
        at.AddExpectedAttribute(def);
        Assert.Contains(at.ExpectedAttributes, e => e.Key == "K");
        Assert.True(at.RemoveExpectedAttribute("K"));
    }

    /// <summary>
    /// Vérifie l'évaluation d'une règle fiscale avec un attribut numérique.
    /// </summary>
    [Fact]
    public void EvaluateTaxRule_WithNumberAttribute_ReturnsValue()
    {
        var at = AssetType.Create("A");
        at.AddExpectedAttribute(AttributeDefinition.Create("ResidualValue", "Valeur Venale", AttributeDataType.Number, true));
        var tr = new TaxRule { Key = "R1", Label = "r1", Expression = "[ResidualValue]*0.01" };
        at.AddTaxRule(tr);

        var attrs = new Collection<ExtendedAttribute> { ExtendedAttribute.Create("ResidualValue", "100", AttributeDataType.Number, true) };
        var result = at.EvaluateTaxRule("R1", attrs);
        Assert.Equal(1m, result);
    }

    /// <summary>
    /// Vérifie le calcul des lignes fiscales pour un actif imposable.
    /// </summary>
    [Fact]
    public void TaxableAsset_CalculateTaxLines_ReturnsLines()
    {
        var at = AssetType.Create("A");
        at.AddExpectedAttribute(AttributeDefinition.Create("ResidualValue", "Valeur Venale", AttributeDataType.Number, true));
        at.AddTaxRule(new TaxRule { Key = "R1", Label = "r1", Expression = "[ResidualValue]*0.01" });

        var attrs = new Collection<ExtendedAttribute> { ExtendedAttribute.Create("ResidualValue", "200", AttributeDataType.Number, true) };
        var asset = TaxableAsset.Create(at, attrs);
        var lines = asset.CalculateTaxLines();
        Assert.Single(lines);
        Assert.Equal(2m, lines.First().Amount);
    }
}
