using EventMessage.Queues;
using MassTransit;

namespace NotificationService.API.Infrastructure.Consumers;

public class PublishNotificationConsumer : IConsumer<PublishNotification>
{
    private readonly ILogger<PublishNotificationConsumer> _logger;

    public PublishNotificationConsumer(ILogger<PublishNotificationConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PublishNotification> context)
    {
        var funcName = $"{nameof(PublishNotificationConsumer)} - {nameof(Consume)} - Correlation = {context.Message.CorrelationId} =>";
        _logger.LogInformation(funcName);
        try
        {
            var id = context.Message.Content.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{funcName} has error: Message = {ex.Message}");
        }
    }
}

internal class PublishNotificationConsumerDefinition : ConsumerDefinition<PublishNotificationConsumer>
{
    public PublishNotificationConsumerDefinition()
    {
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<PublishNotificationConsumer> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Interval(1, TimeSpan.FromSeconds(60)));
    }
}