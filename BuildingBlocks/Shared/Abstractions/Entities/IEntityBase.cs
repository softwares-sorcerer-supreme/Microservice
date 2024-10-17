namespace Shared.Abstractions.Entities;

public interface IEntityBase<TKey>
{
    TKey Id { get; }
}