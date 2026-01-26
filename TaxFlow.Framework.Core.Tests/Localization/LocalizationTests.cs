using System.Globalization;
using Core.Domain.Localization;
using Xunit;

namespace TaxFlow.Framework.Core.Tests.Localization;
/// <summary>
/// Localization tests for LocalizedString and LocalizedTemplate classes.
/// </summary>

public class LocalizationTests
{
    /// <summary>
    /// Tests that LocalizedString returns the default value when no culture is specified.
    /// </summary>
    [Fact]
    public void LocalizedString_ReturnsDefaultValue_WhenCultureNotSpecified()
    {
        var label = LocalizedString.Create("Déclaration");

        Assert.Equal("Déclaration", label.GetValue());
        Assert.Equal("Déclaration", label.GetValue("de-DE"));
    }
    /// <summary>
    /// Tests that LocalizedString returns the correct translation based on the current culture scope.
    /// </summary>
    [Fact]
    public void LocalizedString_UsesTranslations_WhenCultureScopeIsSet()
    {
        var label = LocalizedString.Create("Paiement")
            .En("Payment")
            .Ar("???");

        using (LocalizationContext.WithCulture("en-US"))
        {
            Assert.Equal("Payment", label.GetValue());
        }

        using (LocalizationContext.WithCulture("ar-SA"))
        {
            Assert.Equal("???", label.GetValue());
        }

        using (LocalizationContext.WithCulture("es-ES"))
        {
            Assert.Equal("Paiement", label.GetValue());
        }
    }
    /// <summary>
    /// Tests that LocalizedTemplate formats values according to culture-specific formatting rules.
    /// </summary>
    [Fact]
    public void LocalizedTemplate_Format_UsesCultureSpecificFormatting()
    {
        var template = LocalizedTemplate.Create(
            "Montant dû: {amount}",
            ("en-US", "Amount due: {amount}"));

        var amount = 1234.5m;
        var frExpected = string.Format(CultureInfo.GetCultureInfo("fr-FR"), "{0:N2}", amount);
        var enExpected = string.Format(CultureInfo.GetCultureInfo("en-US"), "{0:N2}", amount);

        var fr = template.Format("fr-FR", ("amount", amount));
        var en = template.Format("en-US", ("amount", amount));

        Assert.Equal($"Montant dû: {frExpected}", fr);
        Assert.Equal($"Amount due: {enExpected}", en);
    }
}
