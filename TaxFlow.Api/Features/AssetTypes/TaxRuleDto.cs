namespace TaxFlow.Api.Features.AssetTypes;

public sealed record TaxRuleDto(
    Guid Id,
    string Key,
    string Label,
    string Expression,
    string? Description,
    bool Enabled);
