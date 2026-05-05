namespace TaxFlow.Api.Features.AssetTypes;

public sealed record CreateTaxRuleRequest(
    string Key,
    string Label,
    string Expression,
    string? Description = null,
    bool Enabled = true);
