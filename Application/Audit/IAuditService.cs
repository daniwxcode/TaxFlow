namespace Application.Audit;
/// <summary>
/// Service dédié à la gestion et à la récupération des pistes d’audit.
/// </summary>
public interface IAuditService
{
    /// <summary>
    /// Récupère la liste des entrées d’audit correspondant aux critères spécifiés dans le filtre.
    /// </summary>
    /// <param name="filter">Filtres optionnels pour affiner la recherche des pistes d’audit.</param>
    /// <returns>Liste des pistes d’audit correspondant aux critères.</returns>
    Task<List<AuditTrailEntity>> GetUserTrailsAsync(AuditFilter filter);
}
