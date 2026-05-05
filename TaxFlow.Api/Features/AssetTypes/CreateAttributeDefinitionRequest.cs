using Core.Domain.Enums;

namespace TaxFlow.Api.Features.AssetTypes;

public sealed record CreateAttributeDefinitionRequest(
    string Key,
    string Label,
    AttributeDataType DataType,
    bool IsRequired = false,
    string? RegexPattern = null,
    EnumDefinitionRequest? EnumDefinition = null);

public sealed record EnumDefinitionRequest(
    string Key,
    string Label,
    IReadOnlyList<EnumItemRequest> Items);

public sealed record EnumItemRequest(
    string Code,
    string Label,
    int Order = 0);
