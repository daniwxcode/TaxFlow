using Core.Domain.Contracts;
using Core.Domain.Enums;
using Core.Domain.Tax.Assets;
using Core.Domain.Tax.Calculation;

namespace Core.Bootstrap.AssetTypes;
/// <summary>
/// Personal Income asset type definition.
/// </summary>
public sealed class PersonalIncomeAssetTypeDefinition : IAssetTypeDefinition
{
    /// <summary>
    /// Gets the unique identifier for this asset type.
    /// </summary>
    public string AssetTypeKey => "PERSONAL_INCOME";
    /// <summary>
    /// Gets the human-readable name of the asset type.
    /// </summary>
    public string Name => "Household Income";
    /// <summary>
    /// Gets the description of the asset type.
    /// </summary>
    public string Description => "Impôts personnels IRF/IRTS/IRPRV/IRCM/IRGM";
    /// <summary>
    /// Gets the liquidation mode for this asset type.
    /// </summary>
    public LiquidationMode LiquidationMode => LiquidationMode.Grouped;

    /// <summary>
    /// Builds the asset type definition.
    /// </summary>
    /// <returns></returns>
    public AssetType Build()
    {
        AssetType assetType = AssetType.Create(Name, Description, LiquidationMode);

        assetType.AddExpectedAttribute(AttributeDefinition.Create(
            "AnnualGlobalIncome", "Revenu global annuel", AttributeDataType.Number));
        assetType.AddExpectedAttribute(AttributeDefinition.Create(
            "PensionAmount", "Pensions et rentes", AttributeDataType.Number));
        assetType.AddExpectedAttribute(AttributeDefinition.Create(
            "CapitalIncomeAmount", "Revenus de capitaux mobiliers", AttributeDataType.Number));
        assetType.AddExpectedAttribute(AttributeDefinition.Create(
            "ManagerRemuneration", "Rémunérations de gérance", AttributeDataType.Number));

        foreach (TaxRule rule in GetTaxRules())
        {
            assetType.AddTaxRule(rule);
        }

        return assetType;
    }

    private IEnumerable<TaxRule> GetTaxRules()
    {
        yield return new TaxRule
        {
            Key = "IRPRV",
            Label = "IMPÔT SUR PENSIONS ET RENTES",
            Description = "Tranches 2,4-3,6M à 25 %, au-delà 50 %.",
            Expression = """
            (
                [PensionAmount]<=2400000?0:
                (
                    [PensionAmount]<=3600000?
                        ([PensionAmount]-2400000)*0.25:
                        (
                            (3600000-2400000)*0.25 + ([PensionAmount]-3600000)*0.50
                        )
                )
            )
            """
        };

        yield return new TaxRule
        {
            Key = "IRTS",
            Label = "IR SUR TRAITEMENTS ET SALAIRES",
            Description = "Barème IRPP sur le revenu global.",
            Expression = BuildIrppScaleExpression("AnnualGlobalIncome")
        };

        yield return new TaxRule
        {
            Key = "IRCM",
            Label = "IMPÔT SUR REVENUS DE CAPITAUX MOBILIERS",
            Description = "Taxe proportionnelle de 15 % sur les revenus agrégés.",
            Expression = "[CapitalIncomeAmount]*0.15"
        };

        yield return new TaxRule
        {
            Key = "IRGM",
            Label = "IMPÔT SUR REVENUS DE GÉRANTS",
            Description = "Taxe proportionnelle de 10 % sur les rémunérations des gérants/associés.",
            Expression = "[ManagerRemuneration]*0.10"
        };
    }

    private static string BuildIrppScaleExpression(string variableName) =>
        $"""
        (
            (([{variableName}]>900000?([{variableName}]<3000000?[{variableName}]:3000000):900000)-900000)*0.10 +
            (([{variableName}]>3000000?([{variableName}]<9000000?[{variableName}]:9000000):3000000)-3000000)*0.15 +
            (([{variableName}]>9000000?([{variableName}]<12000000?[{variableName}]:12000000):9000000)-9000000)*0.20 +
            (([{variableName}]>12000000?([{variableName}]<15000000?[{variableName}]:15000000):12000000)-12000000)*0.25 +
            (([{variableName}]>15000000?([{variableName}]<20000000?[{variableName}]:20000000):15000000)-15000000)*0.30 +
            (([{variableName}]>20000000?[{variableName}]:20000000)-20000000)*0.35
        )
        """;
}
