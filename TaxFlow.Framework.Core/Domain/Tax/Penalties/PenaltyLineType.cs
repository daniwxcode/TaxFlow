namespace Core.Domain.Tax.Penalties;

/// <summary>
/// Line classification for penalties.
/// </summary>
public enum PenaltyLineType
{
    /// <summary>
    /// Fixed assiette penalty line.
    /// </summary>
    AssietteFixed = 1,

    /// <summary>
    /// Assiette penalty line based on rate and periodicity.
    /// </summary>
    AssietteRate = 2,

    /// <summary>
    /// Recouvrement penalty line based on rate and periodicity.
    /// </summary>
    RecouvrementRate = 3
}
