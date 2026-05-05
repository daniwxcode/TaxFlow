using Core.Domain.Tax.Obligations;

namespace TaxFlow.Api.Features.AssetTypes;

public sealed record PaymentDeadlineRequest(
    string Key,
    string Label,
    DateTimeOffset DueDate,
    PaymentType PaymentType,
    decimal Fraction,
    int Order,
    DeadlinePeriodicity Periodicity,
    TaxRegime Regime,
    DurationRequest GracePeriod,
    string? LinkedDeclarationKey,
    bool AllowsPartialPayment,
    decimal? MinimumPayment,
    decimal? FixedAmount,
    PenaltyDefinitionRequest? PenaltyDefinition,
    IReadOnlyList<LegalReferenceRequest>? LegalReferences);
