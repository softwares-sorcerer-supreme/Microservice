namespace EventMessage.IntegrationEvents;

public record SendNotificationEvent
{
    public Guid Id { get; set; }
}