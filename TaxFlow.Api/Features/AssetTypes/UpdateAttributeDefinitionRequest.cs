namespace TaxFlow.Api.Features.AssetTypes;

public sealed record UpdateAttributeDefinitionRequest(
    string Label,
    string? RegexPattern = null);
