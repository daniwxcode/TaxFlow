using System;
using System.Reflection;
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
        // Create EnumDefinition and set internal properties via reflection because setters are internal
        var ed = new EnumDefinition();
        var t = typeof(EnumDefinition);
        t.GetProperty("Key", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!.SetValue(ed, "E");
        t.GetProperty("Label", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!.SetValue(ed, "E");

        // Create EnumItem and set internal properties via reflection
        var item = new EnumItem();
        var ti = typeof(EnumItem);
        ti.GetProperty("Code", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!.SetValue(item, "A");
        ti.GetProperty("Label", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!.SetValue(item, "Alpha");

        ed.Items.Add(item);

        var def = AttributeDefinition.Create(ed);
        Assert.Equal("E", def.Key);
        Assert.Equal(AttributeDataType.Enum, def.DataType);
        Assert.NotNull(def.RegexPattern);
    }
}
