using Core.Domain.Enums;

namespace TaxFlow.Api.Features.AssetTypes;

public sealed record AssetTypeDto(
    Guid Id,
    string Name,
    string? Description,
    LiquidationMode LiquidationMode);
