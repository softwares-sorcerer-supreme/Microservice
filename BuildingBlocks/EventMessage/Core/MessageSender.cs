using MassTransit;
using Microsoft.Extensions.Logging;

namespace EventMessage.Core;

public class MessageSender : IMessageSender
{
    private readonly ILogger<MessageSender> _logger;
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly IPublishEndpoint _publishEndpoint;

    public MessageSender
    (
        ISendEndpointProvider sendEndpointProvider,
        IPublishEndpoint publishEndpoint,
        ILogger<MessageSender> logger
    )
    {
        _sendEndpointProvider = sendEndpointProvider;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task SendMessage<T>(object eventModel, CancellationToken cancellationToken) where T : class
    {
        const string funcName = $"{nameof(MessageSender)} {nameof(SendMessage)} =>";
        try
        {
            _logger.LogDebug($"{funcName} is called ...");
            var kebabFormatter = new KebabCaseEndpointNameFormatter(false);
            var queueName = kebabFormatter.SanitizeName(typeof(T).Name);

            if (!string.IsNullOrWhiteSpace(queueName))
            {
                _logger.LogDebug($"{funcName} QueueName: {queueName}");
                var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{queueName}"));
                await sendEndpoint.Send<T>(eventModel, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{funcName} Has error: {ex.Message}");
        }
    }

    public async Task PublishMessage<T>(object eventModel, CancellationToken cancellationToken) where T : class
    {
        const string funcName = $"{nameof(PublishMessage)} {nameof(SendMessage)} =>";
        try
        {
            _logger.LogDebug($"{funcName} is called ...");
            await _publishEndpoint.Publish<T>(eventModel, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{funcName} Has error: {ex.Message}");
        }
    }
}