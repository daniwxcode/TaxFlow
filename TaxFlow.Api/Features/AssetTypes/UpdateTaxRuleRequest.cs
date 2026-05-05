namespace TaxFlow.Api.Features.AssetTypes;

public sealed record UpdateTaxRuleRequest(
    string Label,
    string Expression,
    string? Description = null,
    bool Enabled = true);
