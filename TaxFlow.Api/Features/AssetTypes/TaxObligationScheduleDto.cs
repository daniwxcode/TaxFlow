namespace TaxFlow.Api.Features.AssetTypes;

public sealed record TaxObligationScheduleDto(
    string? Name,
    string? Description,
    int? FiscalYear,
    IReadOnlyList<DeclarationDeadlineDto> Declarations,
    IReadOnlyList<PaymentDeadlineDto> Payments,
    IReadOnlyList<LegalReferenceDto> LegalReferences);
