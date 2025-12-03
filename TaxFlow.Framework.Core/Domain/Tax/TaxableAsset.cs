using Core.Domain.Contracts.Abstracts;

namespace Core.Domain.Tax;

internal class TaxableAsset: ExtendedTemporalSoftAuditableEntity
{
    public string AssetType { get; set; }
}
