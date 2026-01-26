namespace Application.Audit;

/// <summary>
/// Représente les critères de filtrage pour interroger les entrées d’audit.
/// </summary>
public sealed class AuditFilter
{
    /// <summary>
    /// Date/heure de début (inclus) pour filtrer les entrées d’audit selon leur date d’occurrence.
    /// </summary>
    public DateTimeOffset? FromOccurredAt { get; set; }

    /// <summary>
    /// Date/heure de fin (inclus) pour filtrer les entrées d’audit selon leur date d’occurrence.
    /// </summary>
    public DateTimeOffset? ToOccurredAt { get; set; }

    /// <summary>
    /// Identifiant facultatif de l’utilisateur pour filtrer les entrées réalisées par un utilisateur spécifique.
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Type d’opération (création, mise à jour, suppression, etc.) pour filtrer les entrées par type d’action.
    /// </summary>
    public AuditOperation? Operation { get; set; }

    /// <summary>
    /// Nom de l’entité (table ou domaine) à filtrer.
    /// </summary>
    public string? Entity { get; set; }

    /// <summary>
    /// Fonctionnalité applicative pour filtrer les entrées d’audit issues d’une fonctionnalité spécifique.
    /// </summary>
    public string? Feature { get; set; }
}
