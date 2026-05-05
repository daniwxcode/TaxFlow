namespace TaxFlow.Api.Features.AssetTypes;

public sealed record UpsertTaxObligationScheduleRequest(
    string? Name,
    string? Description,
    int? FiscalYear,
    IReadOnlyList<DeclarationDeadlineRequest> Declarations,
    IReadOnlyList<PaymentDeadlineRequest> Payments,
    IReadOnlyList<LegalReferenceRequest>? LegalReferences);
