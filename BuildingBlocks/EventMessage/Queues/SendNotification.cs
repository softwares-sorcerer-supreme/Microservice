using EventMessage.IntegrationEvents;
using MassTransit;

namespace EventMessage.Queues;

[ConfigureConsumeTopology(false)] //Remove MessageType which only used in .Publish
public interface SendNotification : CorrelatedBy<Guid>
{
    SendNotificationEvent Content { get; }
}