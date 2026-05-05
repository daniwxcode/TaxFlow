using Core.Domain.Enums;

namespace TaxFlow.Api.Features.AssetTypes;

public sealed record AssetCreationModelDto(
    Guid AssetTypeId,
    string AssetTypeName,
    string? Description,
    LiquidationMode LiquidationMode,
    IReadOnlyList<AssetAttributeFieldDto> Attributes);

public sealed record AssetAttributeFieldDto(
    string Key,
    string Label,
    AttributeDataType DataType,
    bool IsRequired,
    string? RegexPattern,
    EnumDefinitionDto? EnumDefinition);
