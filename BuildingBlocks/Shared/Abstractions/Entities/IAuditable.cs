namespace Shared.Abstractions.Entities;

public interface IAuditable : IDateTracking, IUserTracking, ISoftDelete;