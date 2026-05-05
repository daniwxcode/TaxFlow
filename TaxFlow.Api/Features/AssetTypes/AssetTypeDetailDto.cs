using Core.Domain.Enums;

namespace TaxFlow.Api.Features.AssetTypes;

public sealed record AssetTypeDetailDto(
    Guid Id,
    string Name,
    string? Description,
    LiquidationMode LiquidationMode,
    IReadOnlyList<AttributeDefinitionDto> Attributes,
    IReadOnlyList<TaxRuleDto> TaxRules);
