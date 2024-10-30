namespace CartService.Application.Abstractions.Services.EventMessageService;

public interface ISendMessageService
{
    Task SendAddToCartNotification(CancellationToken cancellationToken);
    Task PublishAddToCartNotification(CancellationToken cancellationToken);
}