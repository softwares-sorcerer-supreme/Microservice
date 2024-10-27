namespace EventMessage.Core;

public interface IMessageSender
{
    Task SendMessage<T>(object eventModel, CancellationToken cancellationToken) where T : class;
    Task PublishMessage<T>(object eventModel, CancellationToken cancellationToken) where T : class;
}