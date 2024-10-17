namespace Shared.Abstractions.Entities;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
}