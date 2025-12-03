namespace Core.Domain.Contracts.Abstracts;
public abstract class AuditableEntity : BaseEntity<Guid>, IAuditable
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public DateTimeOffset Created { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTimeOffset LastModified { get; private set; }
    public Guid? LastModifiedBy { get; private set; }
}
