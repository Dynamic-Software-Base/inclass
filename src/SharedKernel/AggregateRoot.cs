using SharedKernel.ValueObjects.StronglyTypedIds;

namespace SharedKernel;

public class AggregateRoot<TSelf , TId> : BaseAuditableEntity<TSelf , TId>,IHasDomainEvents
where TSelf : AggregateRoot<TSelf, TId>
where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();


    public void RaiseDomainEvent(IDomainEvent domainEvent) =>
        _domainEvents.Add(domainEvent);
    public void ClearDomainEvents()=> _domainEvents.Clear();

    protected AggregateRoot(TId id , UserId createdBy) : base(id,createdBy)
    {

    }

    protected AggregateRoot() : base()
    { }
}
