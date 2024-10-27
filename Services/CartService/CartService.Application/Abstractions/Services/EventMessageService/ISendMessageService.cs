namespace CartService.Application.Abstractions.Services.EventMessageService;

public interface ISendMessageService
{
    Task SendAddToCartNotification(CancellationToken cancellationToken);
}