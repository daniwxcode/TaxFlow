using Core.Domain.Tax.Penalties;

namespace TaxFlow.Api.Features.AssetTypes;

public sealed record PenaltyDefinitionRequest(
    PenaltyType Type,
    PenaltyTriggerEvent TriggerEvent,
    decimal FixedAmount,
    DurationRequest GracePeriod,
    DurationRequest Period,
    decimal AnnualRate,
    decimal PeriodRate,
    decimal PeriodRateIncrement,
    decimal? Cap,
    decimal? Minimum,
    bool Capitalize);
