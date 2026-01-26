/// <summary>
/// Représente les options de configuration pour la mise en cache.
/// </summary>
public class CacheOptions
{
    /// <summary>
    /// Durée par défaut pendant laquelle les données sont mises en cache.
    /// </summary>
    public TimeSpan DefaultCacheDuration { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Indique si la mise en cache est activée ou désactivée.
    /// </summary>
    public bool EnableCaching { get; set; } = true;

    /// <summary>
    /// Nom du fournisseur de cache à utiliser (par exemple "InMemory", "Redis").
    /// </summary>
    public CacheProviderType CacheProvider { get; set; } = CacheProviderType.InMemory;

    /// <summary>
    /// Chaîne de connexion utilisée par le fournisseur de cache, si applicable.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;
}
