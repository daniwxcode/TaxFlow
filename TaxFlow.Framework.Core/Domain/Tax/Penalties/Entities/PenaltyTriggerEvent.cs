namespace Core.Domain.Tax.Penalties;

/// <summary>
/// Event that triggers penalty evaluation.
/// </summary>
[Flags]
public enum PenaltyTriggerEvent
{
    /// <summary>
    /// Applies to any event.
    /// </summary>
    Any = 0,

    /// <summary>
    /// Triggered on liquidation.
    /// </summary>
    Liquidation = 1 << 0,

    /// <summary>
    /// Triggered on payment (collection).
    /// </summary>
    Payment = 1 << 1
}
