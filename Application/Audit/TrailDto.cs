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
    public Guid Id { get; set; }

    /// <summary>
    /// Date et heure de l’opération (UTC).
    /// </summary>
    public DateTimeOffset OccurredAt { get; set; }

    public Guid UserId { get; set; }

    /// <summary>
    /// Clé(s) primaire(s) de l’entité affectée.
    /// </summary>
    public Dictionary<string, object?> KeyValues { get; } = new();

    /// <summary>
    /// Valeurs avant modification.
    /// </summary>
    public Dictionary<string, object?> OldValues { get; } = new();

    /// <summary>
    /// Valeurs après modification.
    /// </summary>
    public Dictionary<string, object?> NewValues { get; } = new();

    /// <summary>
    /// Liste des propriétés modifiées.
    /// </summary>
    public Collection<string> ModifiedProperties { get; } = new();

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

    private static readonly JsonSerializerOptions SerializerOptions = new()
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
            Id = Guid.NewGuid(),
            UserId = UserId,
            Operation = Type.ToString(),
            Entity = Entity ?? string.Empty,
            OccurredAt = OccurredAt,
            PrimaryKey = JsonSerializer.Serialize(KeyValues, SerializerOptions),
            PreviousValues = OldValues.Count == 0 ? null : JsonSerializer.Serialize(OldValues, SerializerOptions),
            NewValues = NewValues.Count == 0 ? null : JsonSerializer.Serialize(NewValues, SerializerOptions),
            ModifiedProperties = ModifiedProperties.Count == 0 ? null : JsonSerializer.Serialize(ModifiedProperties, SerializerOptions),
            Feature = Feature,
            FeatureVersion = FeatureVersion,
            UseCase = UseCase,
            Source = Source,
            CorrelationId = CorrelationId
        };
    }
}
