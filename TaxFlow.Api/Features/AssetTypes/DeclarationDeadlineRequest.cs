using Core.Domain.Tax.Obligations;

namespace TaxFlow.Api.Features.AssetTypes;

public sealed record DeclarationDeadlineRequest(
    string Key,
    string Label,
    DateTimeOffset DueDate,
    DeadlinePeriodicity Periodicity,
    TaxRegime Regime,
    int Order,
    DurationRequest GracePeriod,
    DeclarationType DeclarationType,
    bool RequiresDocuments,
    string? FormReference,
    PenaltyDefinitionRequest? PenaltyDefinition,
    IReadOnlyList<LegalReferenceRequest>? LegalReferences);
