using EventMessage.Queues;
using MassTransit;

namespace NotificationService.API.Infrastructure.Consumers;

public class SendNotificationConsumer : IConsumer<SendNotification>
{
    private readonly ILogger<SendNotificationConsumer> _logger;
    
    
    public SendNotificationConsumer(ILogger<SendNotificationConsumer> logger)
    {
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<SendNotification> context)
    {
        var funcName = $"{nameof(SendNotificationConsumer)} - {nameof(Consume)} - Correlation = {context.Message.CorrelationId} =>";
        try
        {
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{funcName} has error: Message = {ex.Message}");
        }
    }
}


class SendNotificationConsumerDefinition :
    ConsumerDefinition<SendNotificationConsumer>
{
    public SendNotificationConsumerDefinition()
    {
        
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<SendNotificationConsumer> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Interval(1, TimeSpan.FromSeconds(60)));
    }
}