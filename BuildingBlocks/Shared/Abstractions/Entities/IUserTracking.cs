namespace Shared.Abstractions.Entities;

public interface IUserTracking
{
    string CreatedBy { get; set; }
    string? ModifiedBy { get; set; }
}