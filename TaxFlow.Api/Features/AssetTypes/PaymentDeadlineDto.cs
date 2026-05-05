using Core.Domain.Tax.Obligations;

namespace TaxFlow.Api.Features.AssetTypes;

public sealed record PaymentDeadlineDto(
    string Key,
    string Label,
    DateTimeOffset DueDate,
    decimal Fraction,
    int Order,
    PaymentType PaymentType,
    DeadlinePeriodicity Periodicity,
    TaxRegime Regime,
    DurationDto GracePeriod,
    string? LinkedDeclarationKey,
    bool AllowsPartialPayment,
    decimal? MinimumPayment,
    decimal? FixedAmount,
    PenaltyDefinitionDto? PenaltyDefinition,
    IReadOnlyList<LegalReferenceDto> LegalReferences);
