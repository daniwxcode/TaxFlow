namespace Core.Domain.Enums;

/// <summary>
/// Defines how tax liquidation should aggregate assets.
/// </summary>
public enum LiquidationMode
{
    /// <summary>
    /// Each asset instance generates its own liquidation line.
    /// </summary>
    Individual = 0,

    /// <summary>
    /// Asset instances are grouped together before liquidation.
    /// </summary>
    Grouped = 1
}
