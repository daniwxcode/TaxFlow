namespace TaxFlow.Api.Features.AssetTypes;

public sealed record AssetAttributeValueRequest(
    string Key,
    string? Value,
    DateTimeOffset? ValidFrom,
    DateTimeOffset? ValidTo);
