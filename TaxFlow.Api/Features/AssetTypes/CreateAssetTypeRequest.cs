using Core.Domain.Enums;

namespace TaxFlow.Api.Features.AssetTypes;

public sealed record CreateAssetTypeRequest(
    string Name,
    string? Description,
    LiquidationMode LiquidationMode = LiquidationMode.Individual);
