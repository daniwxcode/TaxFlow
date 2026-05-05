using Core.Domain.Enums;

namespace TaxFlow.Api.Features.AssetTypes;

public sealed record TaxableAssetDto(
    Guid Id,
    Guid AssetTypeId,
    string? ExternalId,
    DateTimeOffset ValidFrom,
    DateTimeOffset? ValidTo,
    IReadOnlyList<TaxableAssetAttributeDto> Attributes);

public sealed record TaxableAssetAttributeDto(
    string Key,
    string Value,
    AttributeDataType DataType,
    bool IsRequired,
    DateTimeOffset ValidFrom,
    DateTimeOffset? ValidTo);
