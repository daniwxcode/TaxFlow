using Core.Domain.Contracts;
using Core.Domain.Enums;

using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Tax.Bootstrap;

public static class DefaultAssetTypes
{
    // generate a list of default asset type with attributes for bootstrap
    // real Estate
    // cars
    // Incomes
    // activities
    // stocks
    public static IEnumerable<AssetType> InitialData()
    {
        var realEstateUsage = new EnumDefinition
        {
            Key = "RealEstateUsage",
            Label = "Usage d’un bien immobilier",
            Items =
    {
        new EnumItem { Code = "RES", Label = "Résidentiel", Order = 1 },
        new EnumItem { Code = "COM", Label = "Commercial", Order = 2 },
        new EnumItem { Code = "IND", Label = "Industriel", Order = 3 }
    }
        };
        yield return AssetType.Create("Real Estate", "Properties such as residential and commercial buildings.")
    .AddExpectedAttribute(AttributeDefinition.Create("Situation", "Situation", AttributeDataType.String, null))
    .AddExpectedAttribute(AttributeDefinition.Create("Usage", "Usage", AttributeDataType.Enum, realEstateUsage, true))
    .AddExpectedAttribute(AttributeDefinition.Create("AcquisitionDate", "Acquisition Date", AttributeDataType.Date))
    .AddExpectedAttribute(AttributeDefinition.Create("BuildingCompletionDate", "Date de fin des Travaux", AttributeDataType.Date))
    .AddExpectedAttribute(AttributeDefinition.Create("MarketValue", "Market Value", AttributeDataType.Number))
    .AddExpectedAttribute(AttributeDefinition.Create("RentalIncome", "Rental Income", AttributeDataType.Number));

    }
}

