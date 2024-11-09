using EventMessage.Queues;
using MassTransit;

namespace NotificationService.API.Infrastructure.Consumers;

public class PublishNotificationV2Consumer : IConsumer<PublishNotification>
{
    private readonly ILogger<PublishNotificationV2Consumer> _logger;

    public PublishNotificationV2Consumer(ILogger<PublishNotificationV2Consumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PublishNotification> context)
    {
        var funcName = $"{nameof(PublishNotificationV2Consumer)} - {nameof(Consume)} - Correlation = {context.Message.CorrelationId} =>";
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

internal class PublishNotificationConsumerV2Definition : ConsumerDefinition<PublishNotificationConsumer>
{
    public PublishNotificationConsumerV2Definition()
    {
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<PublishNotificationConsumer> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Interval(1, TimeSpan.FromSeconds(60)));
    }
}