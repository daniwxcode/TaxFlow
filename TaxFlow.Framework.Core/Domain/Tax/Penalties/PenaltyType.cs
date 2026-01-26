namespace Core.Domain.Tax.Penalties;

/// <summary>
/// Defines the types of penalties supported by the domain.
/// </summary>
public enum PenaltyType
{
    /// <summary>
    /// Penalty related to declaration or tax base assessment
    /// (assiette fiscale).
    /// </summary>
    Assiette = 1,

    /// <summary>
    /// Penalty related to tax collection or late/non-payment.
    /// </summary>
    Recouvrement = 2
}

