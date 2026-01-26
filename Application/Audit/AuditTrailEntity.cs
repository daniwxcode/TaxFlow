namespace Application.Audit;

/// <summary>
/// Represents a persistent audit trail entry capturing a user action
/// or system operation performed on a domain entity.
/// </summary>
/// <remarks>
/// This entity stores immutable, append-only audit information used for
/// traceability, compliance, diagnostics, and forensic analysis.
/// <para>
/// Audit entries are typically created by cross-cutting infrastructure
/// components (pipelines, middleware) and consumed through read-only features.
/// </para>
/// </remarks>
public sealed class AuditTrailEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the audit record.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who initiated the operation.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the type of operation performed
    /// (e.g. Create, Update, Delete).
    /// </summary>
    public string Operation { get; set; } = default!;

    /// <summary>
    /// Gets or sets the name of the entity affected by the operation.
    /// </summary>
    public string Entity { get; set; } = default!;

    /// <summary>
    /// Gets the serialized values of the entity before the operation.
    /// </summary>
    public string? PreviousValues { get; set; }

    /// <summary>
    /// Gets the serialized values of the entity after the operation.
    /// </summary>
    public string? NewValues { get; set; }

    /// <summary>
    /// Gets the list of properties that were modified during the operation.
    /// </summary>
    public string? ModifiedProperties { get; set; }

    /// <summary>
    /// Gets or sets the primary key of the affected entity instance.
    /// </summary>
    public string PrimaryKey { get; set; } = default!;

    /// <summary>
    /// Gets the date and time at which the operation occurred (UTC).
    /// </summary>
    public DateTimeOffset OccurredAt { get; set; }

    /// <summary>
    /// Gets the functional feature responsible for the operation.
    /// </summary>
    public string Feature { get; set; } = default!;

    /// <summary>
    /// Gets the functional version of the feature.
    /// </summary>
    public string FeatureVersion { get; set; } = default!;

    /// <summary>
    /// Gets the use case or request type that triggered the operation.
    /// </summary>
    public string UseCase { get; set; } = default!;

    /// <summary>
    /// Gets the origin of the operation (API, BackgroundJob, Batch, etc.).
    /// </summary>
    public string Source { get; set; } = default!;

    /// <summary>
    /// Gets the correlation identifier used to link this audit entry
    /// to a broader execution flow.
    /// </summary>
    public string CorrelationId { get; set; } = default!;
}
