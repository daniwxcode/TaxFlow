namespace Application.Attributs;
/// <summary>
/// Marque une Feature applicative afin d’en permettre l’identification,
/// le versionnement fonctionnel et la traçabilité transverse (audit, monitoring).
/// </summary>
/// <remarks>
/// Ce marqueur est utilisé par les pipelines techniques (audit, logging, tracing)
/// pour enrichir automatiquement le contexte d’exécution sans coupler
/// les Features à l’infrastructure.
/// <para>
/// Une Feature non marquée est considérée comme non traçable.
/// </para>
/// </remarks>
/// <param name="name">
/// Nom fonctionnel de la Feature, suivant la convention
/// &lt;Domaine&gt;.&lt;Action&gt;[.&lt;SousAction&gt;].
/// Exemple : <c>TaxPayer.Create</c>.
/// </param>
/// <param name="version">
/// Version fonctionnelle de la Feature (ex : <c>v1</c>, <c>v2</c>).
/// Cette version évolue uniquement en cas de changement métier
/// ou de rupture de comportement fonctionnel.
/// </param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public sealed class FeatureMarkerAttribute(string name, string version) : Attribute
{
    /// <summary>
    /// Nom de la Feature.
    /// </summary>
    public string Name { get; } = name;
    /// <summary>
    /// Version de la Feature.
    /// </summary>
    public string Version { get; } = version;
}
