using EventMessage.IntegrationEvents;
using MassTransit;

namespace EventMessage.Queues;

public interface PublishNotification : CorrelatedBy<Guid>
{
    SendNotificationEvent Content { get; }
}