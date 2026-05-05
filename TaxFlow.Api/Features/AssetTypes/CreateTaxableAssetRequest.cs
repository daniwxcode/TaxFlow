namespace TaxFlow.Api.Features.AssetTypes;

public sealed record CreateTaxableAssetRequest(
    string? ExternalId,
    IReadOnlyList<AssetAttributeValueRequest> Attributes,
    DateTimeOffset? ValidFrom,
    DateTimeOffset? ValidTo);
