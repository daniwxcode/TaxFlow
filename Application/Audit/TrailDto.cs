using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.Audit;

/// <summary>
/// Data Transfer Object représentant une entrée de piste d’audit
/// temporaire avant transformation en <see cref="AuditTrailEntity"/>.
/// </summary>
public class TrailDto
{
    /// <summary>
    /// Identifiant unique de l’entrée de piste d’audit.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Date et heure de l’opération (UTC).
    /// </summary>
    public DateTimeOffset OccurredAt { get; set; } = DateTimeOffset.UtcNow;
    /// <summary>
    /// Identifiant de l’utilisateur à l’origine de l’opération.
    /// </summary>
    public Guid UserId { get; set; } = Guid.Empty;

    /// <summary>
    /// Clé(s) primaire(s) de l’entité affectée.
    /// </summary>
    public Dictionary<string, object?> KeyValues { get; } = [];

    /// <summary>
    /// Valeurs avant modification.
    /// </summary>
    public Dictionary<string, object?> OldValues { get; } = [];

    /// <summary>
    /// Valeurs après modification.
    /// </summary>
    public Dictionary<string, object?> NewValues { get; } = [];

    /// <summary>
    /// Liste des propriétés modifiées.
    /// </summary>
    public Collection<string> ModifiedProperties { get; } = [];

    /// <summary>
    /// Type d’opération (Create, Update, etc.).
    /// </summary>
    public AuditOperation Type { get; set; }

    /// <summary>
    /// Nom de l’entité (table ou domaine).
    /// </summary>
    public string? Entity { get; set; }

    /// <summary>
    /// Fonctionnalité applicative à l’origine de l’opération.
    /// </summary>
    public string Feature { get; set; } = string.Empty;

    /// <summary>
    /// Version de la fonctionnalité applicative.
    /// </summary>
    public string FeatureVersion { get; set; } = string.Empty;

    /// <summary>
    /// Cas d’utilisation ou type de requête déclencheur.
    /// </summary>
    public string UseCase { get; set; } = string.Empty;

    /// <summary>
    /// Origine de l’opération (API, Batch, BackgroundJob, etc.).
    /// </summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// Identifiant de corrélation pour tracer un flux d’exécution.
    /// </summary>
    public string CorrelationId { get; set; } = string.Empty;

    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = false,
        ReferenceHandler = ReferenceHandler.Preserve,
        MaxDepth = 128
    };

    /// <summary>
    /// Convertit le DTO en entité persistante <see cref="AuditTrailEntity"/>.
    /// </summary>
    public AuditTrailEntity ToAuditTrail()
    {
        return new AuditTrailEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = UserId,
            Operation = Type.ToString(),
            Entity = Entity ?? string.Empty,
            OccurredAt = OccurredAt,
            PrimaryKey = JsonSerializer.Serialize(KeyValues, _serializerOptions),
            PreviousValues = OldValues.Count == 0 ? null : JsonSerializer.Serialize(OldValues, _serializerOptions),
            NewValues = NewValues.Count == 0 ? null : JsonSerializer.Serialize(NewValues, _serializerOptions),
            ModifiedProperties = ModifiedProperties.Count == 0 ? null : JsonSerializer.Serialize(ModifiedProperties, _serializerOptions),
            Feature = Feature,
            FeatureVersion = FeatureVersion,
            UseCase = UseCase,
            Source = Source,
            CorrelationId = CorrelationId
        };
    }
}
