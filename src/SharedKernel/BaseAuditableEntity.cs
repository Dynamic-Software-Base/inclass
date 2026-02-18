using SharedKernel.ValueObjects.StronglyTypedIds;

namespace SharedKernel;

public abstract class BaseAuditableEntity <TSelf,TId> : Entity<TSelf,TId>
where TSelf: BaseAuditableEntity<TSelf,TId>
where TId : notnull
{

    public DateTimeOffset CreatedAt { get; set; }
    public UserId CreatedBy { get; set; }
    public DateTimeOffset LastModifiedAt { get; set; }
    public UserId LastModifiedBy { get; set; }

    public void SetUpdated(DateTimeOffset utcNow, UserId updatedBy)
    {
        LastModifiedAt = utcNow;
        LastModifiedBy = updatedBy;
    }

    protected BaseAuditableEntity(){}

    protected BaseAuditableEntity(TId id , UserId createdBy) : base(id)
    {
        CreatedAt = DateTimeOffset.UtcNow;
        CreatedBy = createdBy;
    }
}
