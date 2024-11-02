using CartService.Application.Abstractions.Services.EventMessageService;
using EventMessage.Core;
using EventMessage.IntegrationEvents;
using EventMessage.Queues;
using Microsoft.Extensions.Logging;

namespace CartService.Infrastructure.Services;

public class SendMessageService : ISendMessageService
{
    private readonly ILogger<SendMessageService> _logger;
    private readonly IMessageSender _messageSender;

    public SendMessageService
    (
        IMessageSender messageSender,
        ILogger<SendMessageService> logger
    )
    {
        _messageSender = messageSender;
        _logger = logger;
    }

    public async Task SendAddToCartNotification(CancellationToken cancellationToken)
    {
        const string funcName = $"{nameof(SendAddToCartNotification)} =>";
        _logger.LogInformation(funcName);
        try
        {
            var id = Guid.NewGuid();
            await _messageSender.SendMessage<SendNotification>(new
            {
                Content = new SendNotificationEvent
                {
                    Id = id,
                }
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{funcName} => {ex.Message}");
        }
    }

    public async Task PublishAddToCartNotification(CancellationToken cancellationToken)
    {
        const string funcName = $"{nameof(PublishAddToCartNotification)} =>";
        _logger.LogInformation(funcName);
        try
        {
            var id = Guid.NewGuid();
            await _messageSender.PublishMessage<PublishNotification>(new
            {
                Content = new SendNotificationEvent
                {
                    Id = id,
                }
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{funcName} => {ex.Message}");
        }
    }
    
}