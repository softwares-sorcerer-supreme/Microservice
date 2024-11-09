using CartService.Application.Abstractions.Services.EventMessageService;
using CartService.Application.Models.Response.CartItems;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace CartService.Application.UseCases.v1.Commands.CartItemCommands.AddItemToCart.PostProcessor;

public class AddItemToCartPostProcessor : IRequestPostProcessor<AddItemToCartCommand, AddItemToCartResponse>
{
    private readonly ILogger<AddItemToCartPostProcessor> _logger;
    private readonly ISendMessageService _sendMessageService;

    public AddItemToCartPostProcessor
    (
        ILogger<AddItemToCartPostProcessor> logger,
        ISendMessageService sendMessageService
    )
    {
        _logger = logger;
        _sendMessageService = sendMessageService;
    }

    public async Task Process(AddItemToCartCommand request, AddItemToCartResponse response, CancellationToken cancellationToken)
    {
        const string functionName = $"{nameof(AddItemToCartPostProcessor)} =>";
        _logger.LogInformation($"{functionName}");
        try
        {
            await _sendMessageService.SendAddToCartNotification(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{functionName} has error: Message = {ex.Message}");
        }
    }
}