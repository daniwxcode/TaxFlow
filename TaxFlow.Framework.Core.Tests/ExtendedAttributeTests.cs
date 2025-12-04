using Core.Domain.Contracts;
using Core.Domain.Enums;
using Xunit;

namespace TaxFlow.Framework.Core.Tests;

public class ExtendedAttributeTests
{
    [Fact]
    public void Create_WithValidParameters_SetsProperties()
    {
        var a = ExtendedAttribute.Create("K","123", AttributeDataType.Number, true);
        Assert.Equal("K", a.Key);
        Assert.Equal("123", a.Value);
        Assert.Equal(AttributeDataType.Number, a.DataType);
        Assert.True(a.IsRequired);
    }

    [Fact]
    public void Create_WithEmptyKey_Throws()
    {
        Assert.Throws<ArgumentException>(() => ExtendedAttribute.Create("","v", AttributeDataType.String));
    }

    [Theory]
    [InlineData("123", true)]
    [InlineData("abc", false)]
    public void IsValidValue_NumberChecks(string value, bool expected)
    {
        var a = ExtendedAttribute.Create("K", value, AttributeDataType.Number);
        Assert.Equal(expected, a.IsValidValue());
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", true)]
    [InlineData("x", false)]
    public void IsValidValue_BooleanChecks(string value, bool expected)
    {
        var a = ExtendedAttribute.Create("K", value, AttributeDataType.Boolean);
        Assert.Equal(expected, a.IsValidValue());
    }

    [Theory]
    [InlineData("2020-01-01", true)]
    [InlineData("notadate", false)]
    public void IsValidValue_DateChecks(string value, bool expected)
    {
        var a = ExtendedAttribute.Create("K", value, AttributeDataType.Date);
        Assert.Equal(expected, a.IsValidValue());
    }

    [Fact]
    public void UpdateValue_ChangesFields()
    {
        var a = ExtendedAttribute.Create("K","1", AttributeDataType.Number);
        a.UpdateValue("2", AttributeDataType.Number, true);
        Assert.Equal("2", a.Value);
        Assert.True(a.IsRequired);
    }
}
