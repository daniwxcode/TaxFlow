using Core.Domain.Tax.Penalties;

namespace TaxFlow.Api.Features.AssetTypes;

public sealed record DurationDto(int Value, TimeUnit Unit);
