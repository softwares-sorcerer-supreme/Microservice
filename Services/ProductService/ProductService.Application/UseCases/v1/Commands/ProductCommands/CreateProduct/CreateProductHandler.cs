using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Application.Models.Response.Products;
using ProductService.Domain.Abstraction;
using ProductService.Domain.Entities;
using Shared.CommonExtension;
using Shared.Models.Response;

namespace ProductService.Application.UseCases.v1.Commands.ProductCommands.CreateProduct;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, CreateProductResponse>
{
    private readonly ILogger<CreateProductHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    public CreateProductHandler(ILogger<CreateProductHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        const string functionName = $"{nameof(CreateProductHandler)} Handler => ";
        var payload = request.Payload;
        var response = new CreateProductResponse
        {
            Status = ResponseStatusCode.OK.ToInt()
        };

        _logger.LogInformation($"{functionName}");

        try
        {
            var queryable = _unitOfWork.Product.GetQueryable();

            var product = await queryable.Where(x => x.Name == payload.Name && !x.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (product != null)
            {
                _logger.LogWarning($"{functionName} Product already exists");
                response.Status = ResponseStatusCode.BadRequest.ToInt();
                response.ErrorMessage = "Product already exists";
                //response.ErrorMessageCode = ResponseStatusCode.BadRequest.ToInt();

                return response;
            }

            var productEntity = new Product 
            {
                IsDeleted = false,
                Name = payload.Name,
                Price = payload.Price,
                Quantity = payload.Quantity,
            };
            
            await _unitOfWork.Product.AddAsync(productEntity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{functionName} Has error => {ex.Message}");
            response.Status = ResponseStatusCode.InternalServerError.ToInt();
            response.ErrorMessage = $"Some error has occurred!";
            //response.ErrorMessageCode = ResponseStatusCode.BadRequest.ToInt();

            return response;
        }

        return response;
    }
}
