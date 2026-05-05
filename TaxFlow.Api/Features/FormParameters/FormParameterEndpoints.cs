using System.Text.RegularExpressions;

using Core.Domain.Enums;
using Core.Domain.Tax.Obligations;
using Core.Domain.Tax.Penalties;
using Core.Domain.Tax.Payments;

namespace TaxFlow.Api.Features.FormParameters;

public static class FormParameterEndpoints
{
    public static IEndpointRouteBuilder MapFormParameterEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/form-params");

        group.MapGet("/asset-types/liquidation-modes", () => Results.Ok(ToParams<LiquidationMode>()));
        group.MapGet("/asset-types/attribute-data-types", () => Results.Ok(ToAttributeDataTypes()));

        group.MapGet("/obligations/deadline-periodicities", () => Results.Ok(ToParams<DeadlinePeriodicity>()));
        group.MapGet("/obligations/deadline-types", () => Results.Ok(ToParams<DeadlineType>()));
        group.MapGet("/obligations/tax-regimes", () => Results.Ok(ToParams<TaxRegime>()));
        group.MapGet("/obligations/payment-types", () => Results.Ok(ToParams<PaymentType>()));
        group.MapGet("/obligations/declaration-types", () => Results.Ok(ToParams<DeclarationType>()));
        group.MapGet("/obligations/legal-text-types", () => Results.Ok(ToParams<LegalTextType>()));

        group.MapGet("/penalties/penalty-types", () => Results.Ok(ToParams<PenaltyType>()));
        group.MapGet("/penalties/penalty-trigger-events", () => Results.Ok(ToParams<PenaltyTriggerEvent>()));
        group.MapGet("/penalties/penalty-line-types", () => Results.Ok(ToParams<PenaltyLineType>()));
        group.MapGet("/penalties/time-units", () => Results.Ok(ToParams<TimeUnit>()));

        group.MapGet("/payments/allocation-strategies", () => Results.Ok(ToParams<AllocationStrategy>()));

        group.MapGet("/all", () => Results.Ok(new
        {
            assetTypes = new
            {
                liquidationModes = ToParams<LiquidationMode>(),
                attributeDataTypes = ToAttributeDataTypes()
            },
            obligations = new
            {
                deadlinePeriodicities = ToParams<DeadlinePeriodicity>(),
                deadlineTypes = ToParams<DeadlineType>(),
                taxRegimes = ToParams<TaxRegime>(),
                paymentTypes = ToParams<PaymentType>(),
                declarationTypes = ToParams<DeclarationType>(),
                legalTextTypes = ToParams<LegalTextType>()
            },
            penalties = new
            {
                penaltyTypes = ToParams<PenaltyType>(),
                penaltyTriggerEvents = ToParams<PenaltyTriggerEvent>(),
                penaltyLineTypes = ToParams<PenaltyLineType>(),
                timeUnits = ToParams<TimeUnit>()
            },
            payments = new
            {
                allocationStrategies = ToParams<AllocationStrategy>()
            }
        }));

        return endpoints;
    }

    private static IReadOnlyList<FormParamDto> ToParams<TEnum>() where TEnum : struct, Enum
    {
        return Enum.GetValues<TEnum>()
            .Select(value =>
            {
                var code = value.ToString();
                var label = ToLabel(code);
                var numericValue = Convert.ToInt32(value);
                return new FormParamDto(code, label, numericValue);
            })
            .ToList();
    }

    private static string ToLabel(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var withSpaces = Regex.Replace(value, "([a-z])([A-Z])", "$1 $2");
        return withSpaces.Replace("_", " ");
    }

    private static IReadOnlyList<FormParamDto> ToAttributeDataTypes()
    {
        return AttributeDataType.List
            .Select(item => new FormParamDto(item.Name, ToLabel(item.Name), item.Value))
            .ToList();
    }
}
