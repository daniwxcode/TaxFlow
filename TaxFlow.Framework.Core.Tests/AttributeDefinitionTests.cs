using Core.Domain.Contracts;
using Core.Domain.Enums;
using Xunit;

namespace TaxFlow.Framework.Core.Tests;

public class AttributeDefinitionTests
{
    [Fact]
    public void Create_WithValidParameters_Succeeds()
    {
        var def = AttributeDefinition.Create("Key","Label", AttributeDataType.String, true);
        Assert.Equal("Key", def.Key);
        Assert.Equal("Label", def.Label);
        Assert.True(def.IsRequired);
        Assert.Equal(AttributeDataType.String, def.DataType);
    }

    [Fact]
    public void Create_WithEmptyKey_Throws()
    {
        Assert.Throws<ArgumentException>(() => AttributeDefinition.Create("","Label", AttributeDataType.String));
    }

    [Fact]
    public void Create_WithEnumDefinition_SetsRegex()
    {
        var ed = new EnumDefinition { Key = "E", Label = "E" };
        ed.Items.Add(new EnumItem { Code = "A", Label = "Alpha" });
        var def = AttributeDefinition.Create(ed);
        Assert.Equal("E", def.Key);
        Assert.Equal(AttributeDataType.Enum, def.DataType);
        Assert.NotNull(def.RegexPattern);
    }
}
