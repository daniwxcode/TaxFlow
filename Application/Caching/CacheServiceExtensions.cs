namespace Application.Caching
{
    /// <summary>
    /// Méthodes d’extension pour faciliter l’utilisation du service de cache.
    /// </summary>
    public static class CacheServiceExtensions
    {
        /// <summary>
        /// Récupère un élément en cache ou l’ajoute si absent (version synchrone).
        /// </summary>
        /// <typeparam name="T">Type de l’élément.</typeparam>
        /// <param name="cache">Service de cache.</param>
        /// <param name="key">Clé unique de l’élément.</param>
        /// <param name="getItemCallback">Fonction pour obtenir l’élément si absent du cache.</param>
        /// <param name="slidingExpiration">Durée d’expiration glissante optionnelle.</param>
        /// <returns>L’élément mis en cache ou récupéré via la fonction callback.</returns>
        public static T? GetOrSet<T>(this ICacheService cache, string key, Func<T?> getItemCallback, TimeSpan? slidingExpiration = null)
        {
            T? value = cache.Get<T>(key);

            if (value is not null)
            {
                return value;
            }

            value = getItemCallback();

            if (value is not null)
            {
                cache.Set(key, value, slidingExpiration);
            }

            return value;
        }

        /// <summary>
        /// Récupère un élément en cache ou l’ajoute si absent (version asynchrone).
        /// </summary>
        /// <typeparam name="T">Type de l’élément.</typeparam>
        /// <param name="cache">Service de cache.</param>
        /// <param name="key">Clé unique de l’élément.</param>
        /// <param name="task">Fonction asynchrone pour obtenir l’élément si absent du cache.</param>
        /// <param name="slidingExpiration">Durée d’expiration glissante optionnelle.</param>
        /// <param name="cancellationToken">Jeton d’annulation.</param>
        /// <returns>L’élément mis en cache ou récupéré via la fonction asynchrone.</returns>
        public static async Task<T?> GetOrSetAsync<T>(this ICacheService cache, string key, Func<Task<T>> task, TimeSpan? slidingExpiration = null, CancellationToken cancellationToken = default)
        {
            T? value = await cache.GetAsync<T>(key, cancellationToken);

            if (value is not null)
            {
                return value;
            }

            value = await task();

            if (value is not null)
            {
                await cache.SetAsync(key, value, slidingExpiration, cancellationToken);
            }

            return value;
        }
    }
}
