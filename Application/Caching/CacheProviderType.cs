/// <summary>
/// Specifies the available types of cache providers that can be used for caching operations.
/// </summary>
/// <remarks>Use this enumeration to select the underlying caching technology when configuring caching behavior in
/// an application. The choice of provider may affect performance, scalability, and deployment requirements.</remarks>
public enum CacheProviderType
{
    /// <summary>
    /// Represents an in-memory data store or resource.
    /// </summary>
    InMemory,
    /// <summary>
    /// Represents a Redis data store or resource.
    /// </summary>
    Redis,
    /// <summary>
    /// Represents a Memcached data store or resource.
    /// </summary>
    Memcached
}
