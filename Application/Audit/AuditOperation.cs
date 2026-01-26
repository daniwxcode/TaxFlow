namespace Application.Audit;

/// <summary>
/// Defines the type of operation being recorded in the audit log.
/// </summary>
/// <remarks>
/// This enumeration is used to classify audit events according to the action
/// performed on the system or its data. Selecting the appropriate value ensures
/// traceability, compliance, and meaningful audit trails for both technical and
/// business reviews.
/// </remarks>
public enum AuditOperation
{
    /// <summary>
    /// Indicates that the audit operation type is not explicitly specified
    /// and should be determined automatically by the system.
    /// </summary>
    AutoGenerate = 0,

    /// <summary>
    /// Represents the creation of a new entity or resource.
    /// </summary>
    Create = 1,

    /// <summary>
    /// Represents the modification or update of an existing entity or resource.
    /// </summary>
    Update = 2,

    /// <summary>
    /// Represents the deletion or removal of an existing entity or resource.
    /// </summary>
    Delete = 3,

    /// <summary>
    /// Represents a read-only access to data without modification.
    /// </summary>
    Read = 4,

    /// <summary>
    /// Represents a user authentication (login) action.
    /// </summary>
    Login = 5,

    /// <summary>
    /// Represents a user session termination (logout) action.
    /// </summary>
    Logout = 6,

    /// <summary>
    /// Represents a change to a user's authentication credentials,
    /// such as a password reset or update.
    /// </summary>
    PasswordChange = 7,

    /// <summary>
    /// Represents a modification to user roles, permissions,
    /// or access rights.
    /// </summary>
    PermissionChange = 8,

    /// <summary>
    /// Represents the export of data from the system to an external format or destination.
    /// </summary>
    DataExport = 9,

    /// <summary>
    /// Represents the import of data into the system from an external source.
    /// </summary>
    DataImport = 10
}
