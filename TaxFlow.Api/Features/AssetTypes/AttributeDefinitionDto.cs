using Core.Domain.Enums;

namespace TaxFlow.Api.Features.AssetTypes;

public sealed record AttributeDefinitionDto(
    Guid Id,
    string Key,
    string Label,
    AttributeDataType DataType,
    bool IsRequired,
    string? RegexPattern,
    EnumDefinitionDto? EnumDefinition);
