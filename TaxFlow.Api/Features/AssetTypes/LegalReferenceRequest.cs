using Core.Domain.Tax.Obligations;

namespace TaxFlow.Api.Features.AssetTypes;

public sealed record LegalReferenceRequest(
    LegalTextType TextType,
    string Reference,
    string Title,
    string? Article,
    DateOnly? PublicationDate,
    DateOnly? EffectiveDate,
    string? Url,
    string? Notes);
