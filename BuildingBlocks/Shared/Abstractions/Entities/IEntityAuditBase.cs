namespace Shared.Abstractions.Entities;

public interface IEntityAuditBase<TKey> : IEntityBase<TKey>, IAuditable;