using AutoMapper;
using CartService.Application.Models.Response.CartItems;
using CartService.Domain.Abstraction;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.CommonExtension;
using Shared.Models.Response;

namespace CartService.Application.UseCases.v1.Commands.CartItemCommands.AddItemToCart;

public class AddItemToCartHandler : IRequestHandler<AddItemToCartCommand, CreateCartItemResponse>
{
    private readonly ILogger<AddItemToCartHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public AddItemToCartHandler(ILogger<AddItemToCartHandler> logger, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CreateCartItemResponse> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
    {
        const string functionName = $"{nameof(AddItemToCartHandler)} => ";
        var payload = request.Payload;
        var response = new CreateCartItemResponse
        {
            Status = ResponseStatusCode.OK.ToInt()
        };

        _logger.LogInformation($"{functionName}");

        try
        {
            var cartQueryable = _unitOfWork.Cart.GetQueryable();
            var cartItemQueryable = _unitOfWork.CartItem.GetQueryable();

            var cart = await cartQueryable.Where(x => x.Id == request.CartId)
                .FirstOrDefaultAsync(cancellationToken);

            if (cart != null)
            {
                _logger.LogWarning($"{functionName} Cart does not exists");
                response.Status = ResponseStatusCode.BadRequest.ToInt();
                response.ErrorMessage = "Cart does not exists";
                //response.ErrorMessageCode = ResponseStatusCode.BadRequest.ToInt();

                return response;
            }
            
            
            
            // var productEntity = _mapper.Map<Product>(payload);
            // await _unitOfWork.Product.AddAsync(productEntity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{functionName} Has error => {ex.Message}");
            response.Status = ResponseStatusCode.InternalServerError.ToInt();
            response.ErrorMessage = $"{functionName} Some error has occured!";
            //response.ErrorMessageCode = ResponseStatusCode.BadRequest.ToInt();

            return response;
        }

        return response;
    }
}
