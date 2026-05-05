using Core.Domain.Tax.Penalties;

namespace TaxFlow.Api.Features.AssetTypes;

public sealed record DurationRequest(int Value, TimeUnit Unit);
