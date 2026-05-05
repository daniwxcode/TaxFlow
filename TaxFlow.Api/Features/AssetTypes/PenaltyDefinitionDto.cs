using Core.Domain.Tax.Penalties;

namespace TaxFlow.Api.Features.AssetTypes;

public sealed record PenaltyDefinitionDto(
    PenaltyType Type,
    PenaltyTriggerEvent TriggerEvent,
    decimal FixedAmount,
    DurationDto GracePeriod,
    DurationDto Period,
    decimal AnnualRate,
    decimal PeriodRate,
    decimal PeriodRateIncrement,
    decimal? Cap,
    decimal? Minimum,
    bool Capitalize);
