using Shared.Abstractions.Entities;

namespace Shared.Abstractions;

public abstract class EntityBase<TKey> : IEntityBase<TKey>
{
    public TKey Id { get; set; }
}