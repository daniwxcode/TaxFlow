namespace Core.Domain.Contracts.Abstracts;
public abstract class AuditableEntity : BaseEntity<Guid>, IAuditable
{
    public DateTimeOffset Created { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTimeOffset LastModified { get; private set; }
    public Guid? LastModifiedBy { get; private set; }
}
