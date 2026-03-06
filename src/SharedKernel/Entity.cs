using System.Diagnostics.CodeAnalysis;

namespace SharedKernel;

[SuppressMessage("Major Code Smell", "S4035:Seal class 'Entity' or implement 'IEqualityComparer<T>' instead.",
    Justification = "DDD entity base: equality is defined by Id and runtime type.")]
public abstract class Entity<TSelf,TId> : IEquatable<TSelf>
where TSelf :Entity<TSelf,TId>
where TId : notnull
{
    public TId Id { get; } = default!;
    protected Entity(TId id) => Id = id;
    protected Entity(){}


    public  bool Equals(TSelf? other)
        => other is not null
           && (ReferenceEquals(this, other) || (GetType() == other.GetType() && Id.Equals(other.Id)));


    public override bool Equals(object? obj) => obj is TSelf other && Equals(other);
    public override int GetHashCode()
    {
        return HashCode.Combine(GetType(), Id);
    }
}
