namespace Application.Caching;

/// <summary>
/// Interface définissant les opérations de gestion du cache, 
/// incluant les méthodes synchrones et asynchrones.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Récupère un élément en cache par sa clé (synchronique).
    /// </summary>
    /// <typeparam name="T">Type de l’élément.</typeparam>
    /// <param name="key">Clé unique de l’élément.</param>
    /// <returns>L’élément mis en cache ou <c>null</c> s’il n’existe pas.</returns>
    T? Get<T>(string key);

    /// <summary>
    /// Récupère un élément en cache par sa clé (asynchronique).
    /// </summary>
    /// <typeparam name="T">Type de l’élément.</typeparam>
    /// <param name="key">Clé unique de l’élément.</param>
    /// <param name="token">Jeton d’annulation.</param>
    /// <returns>L’élément mis en cache ou <c>null</c> s’il n’existe pas.</returns>
    Task<T?> GetAsync<T>(string key, CancellationToken token = default);

    /// <summary>
    /// Actualise l’entrée en cache associée à la clé (synchronique).
    /// </summary>
    /// <param name="key">Clé de l’élément à actualiser.</param>
    void Refresh(string key);

    /// <summary>
    /// Actualise l’entrée en cache associée à la clé (asynchrone).
    /// </summary>
    /// <param name="key">Clé de l’élément à actualiser.</param>
    /// <param name="token">Jeton d’annulation.</param>
    Task RefreshAsync(string key, CancellationToken token = default);

    /// <summary>
    /// Supprime un élément du cache par sa clé (synchronique).
    /// </summary>
    /// <param name="key">Clé de l’élément à supprimer.</param>
    void Remove(string key);

    /// <summary>
    /// Supprime plusieurs éléments du cache par leurs clés (synchronique).
    /// </summary>
    /// <param name="keys">Clés des éléments à supprimer.</param>
    void Remove(params string[] keys);

    /// <summary>
    /// Supprime un élément du cache par sa clé (asynchrone).
    /// </summary>
    /// <param name="key">Clé de l’élément à supprimer.</param>
    /// <param name="token">Jeton d’annulation.</param>
    Task RemoveAsync(string key, CancellationToken token = default);

    /// <summary>
    /// Supprime plusieurs éléments du cache par leurs clés (asynchrone).
    /// </summary>
    /// <param name="keys">Clés des éléments à supprimer.</param>
    /// <param name="token">Jeton d’annulation.</param>
    Task RemoveAsync(string[] keys, CancellationToken token = default);

    /// <summary>
    /// Ajoute ou met à jour un élément dans le cache avec une durée d’expiration glissante (synchronique).
    /// </summary>
    /// <typeparam name="T">Type de l’élément.</typeparam>
    /// <param name="key">Clé unique de l’élément.</param>
    /// <param name="value">Valeur à stocker.</param>
    /// <param name="slidingExpiration">Durée d’expiration glissante (optionnelle).</param>
    void Set<T>(string key, T value, TimeSpan? slidingExpiration = null);

    /// <summary>
    /// Ajoute ou met à jour un élément dans le cache avec une durée d’expiration glissante (asynchrone).
    /// </summary>
    /// <typeparam name="T">Type de l’élément.</typeparam>
    /// <param name="key">Clé unique de l’élément.</param>
    /// <param name="value">Valeur à stocker.</param>
    /// <param name="slidingExpiration">Durée d’expiration glissante (optionnelle).</param>
    /// <param name="cancellationToken">Jeton d’annulation.</param>
    Task SetAsync<T>(string key, T value, TimeSpan? slidingExpiration = null, CancellationToken cancellationToken = default);
}
