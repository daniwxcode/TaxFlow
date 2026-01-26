using System;
using Core.Domain.Contracts;
using Core.Domain.Enums;
using Xunit;

namespace TaxFlow.Framework.Core.Tests;
/// <summary>
/// Teste la classe ExtendedAttribute.
/// </summary>
public class ExtendedAttributeTests
{
    /// <summary>
    /// Teste que la méthode Create initialise correctement les propriétés.
    /// </summary>
    [Fact]
    public void Create_WithValidParameters_SetsProperties()
    {
        ExtendedAttribute a = ExtendedAttribute.Create("K","123", AttributeDataType.Number, true);
        Assert.Equal("K", a.Key);
        Assert.Equal("123", a.Value);
        Assert.Equal(AttributeDataType.Number, a.DataType);
        Assert.True(a.IsRequired);
    }
    /// <summary>
    /// Teste que la méthode Create lance une exception pour une clé vide.
    /// </summary>
    [Fact]
    public void Create_WithEmptyKey_Throws()
    {
        Assert.Throws<ArgumentException>(() => ExtendedAttribute.Create("","v", AttributeDataType.String));
    }
    /// <summary>
    /// Teste la méthode IsValidValue pour différents types de données.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="expected"></param>
    [Theory]
    [InlineData("123", true)]
    [InlineData("abc", false)]
    public void IsValidValue_NumberChecks(string value, bool expected)
    {
        ExtendedAttribute a = ExtendedAttribute.Create("K", value, AttributeDataType.Number);
        Assert.Equal(expected, a.IsValidValue());
    }
    /// <summary>
    /// Teste la méthode IsValidValue pour le type Boolean.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="expected"></param>
    [Theory]
    [InlineData("true", true)]
    [InlineData("false", true)]
    [InlineData("x", false)]
    public void IsValidValue_BooleanChecks(string value, bool expected)
    {
        ExtendedAttribute a = ExtendedAttribute.Create("K", value, AttributeDataType.Boolean);
        Assert.Equal(expected, a.IsValidValue());
    }
    /// <summary>
    /// Teste la méthode IsValidValue pour le type Date.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="expected"></param>
    [Theory]
    [InlineData("2020-01-01", true)]
    [InlineData("notadate", false)]
    public void IsValidValue_DateChecks(string value, bool expected)
    {
        ExtendedAttribute a = ExtendedAttribute.Create("K", value, AttributeDataType.Date);
        Assert.Equal(expected, a.IsValidValue());
    }
    /// <summary>
    /// Tests that UpdateValue correctly updates the Value, DataType, and IsRequired fields.
    /// </summary>
    [Fact]
    public void UpdateValue_ChangesFields()
    {
        ExtendedAttribute a = ExtendedAttribute.Create("K","1", AttributeDataType.Number);
        a.UpdateValue("2", AttributeDataType.Number, true);
        Assert.Equal("2", a.Value);
        Assert.True(a.IsRequired);
    }
}
