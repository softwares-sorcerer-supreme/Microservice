using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Application.Models.Response.Categories;
using ProductService.Domain.Abstraction;
using ProductService.Domain.Entities;
using Shared.CommonExtension;
using Shared.Models.Response;

namespace ProductService.Application.UseCases.v1.Commands.ProductCommands.CreateProduct;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, CreateProductResponse>
{
    private readonly ILogger<CreateProductHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public CreateProductHandler(ILogger<CreateProductHandler> logger, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
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

            var product = await queryable.Where(x => x.Name == payload.Name)
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
            
            var productEntity = _mapper.Map<Product>(payload);
            await _unitOfWork.Product.AddAsync(productEntity, cancellationToken);
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
