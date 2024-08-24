using CartService.Application.Models.Response.CartItems;
using CartService.Domain.Abstraction;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.CommonExtension;
using Shared.Models.Response;

namespace CartService.Application.UseCases.v1.Queries.CartItemQueries.GetItemsByCartId;

public class GetItemsByCartIdHandler : IRequestHandler<GetItemsByCartIdQuery, GetItemsByCartIdResponse>
{
    private readonly ILogger<GetItemsByCartIdHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GetItemsByCartIdHandler(ILogger<GetItemsByCartIdHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<GetItemsByCartIdResponse> Handle(GetItemsByCartIdQuery request, CancellationToken cancellationToken)
    {
        const string functionName = $"{nameof(GetItemsByCartIdHandler)} => ";
        var cartId = request.CartId;
        var response = new GetItemsByCartIdResponse
        {
            Status = ResponseStatusCode.OK.ToInt()
        };

        _logger.LogInformation($"{functionName}");

        try
        {
            var cartQueryable = _unitOfWork.Cart.GetQueryable();
            var cartItemsQueryable = _unitOfWork.CartItem.GetQueryable();

            var cart = await cartQueryable.Where(c => c.Id == cartId).FirstOrDefaultAsync(cancellationToken);
            if (cart != null)
            {
                _logger.LogWarning($"{functionName} Cart does not exists");
                response.Status = ResponseStatusCode.NotFound.ToInt();
                response.ErrorMessage = "Cart does not exists";
                //response.ErrorMessageCode = ResponseStatusCode.BadRequest.ToInt();

                return response;
            }

            var cartItems = await cartItemsQueryable.Where(ci => ci.CartId == cartId)
                .ToListAsync(cancellationToken);

            if (cartItems.Count > 0)
            {
                
            }
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GetItemsByCartIdHandler)} Has error => {ex.Message}");
            response.Status = ResponseStatusCode.InternalServerError.ToInt();
            response.ErrorMessage = "Something went wrong";
            //response.ErrorMessageCode = ""
            return response;
        }
    }
}