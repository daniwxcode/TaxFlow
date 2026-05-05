using Core.Domain.Tax.Obligations;

namespace TaxFlow.Api.Features.AssetTypes;

public sealed record DeclarationDeadlineDto(
    string Key,
    string Label,
    DateTimeOffset DueDate,
    DurationDto GracePeriod,
    DeadlinePeriodicity Periodicity,
    TaxRegime Regime,
    int Order,
    DeclarationType DeclarationType,
    bool RequiresDocuments,
    string? FormReference,
    PenaltyDefinitionDto? PenaltyDefinition,
    IReadOnlyList<LegalReferenceDto> LegalReferences);
