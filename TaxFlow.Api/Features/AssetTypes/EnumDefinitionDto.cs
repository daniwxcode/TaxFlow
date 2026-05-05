namespace TaxFlow.Api.Features.AssetTypes;

public sealed record EnumDefinitionDto(
    string Key,
    string Label,
    IReadOnlyList<EnumItemDto> Items);

public sealed record EnumItemDto(
    string Code,
    string Label,
    int Order);
