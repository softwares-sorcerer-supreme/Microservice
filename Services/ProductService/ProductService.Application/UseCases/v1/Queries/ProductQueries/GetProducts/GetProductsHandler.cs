﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Application.Models.Response.Products;
using ProductService.Domain.Abstraction;
using Shared.CommonExtension;
using Shared.Extensions;
using Shared.Models.Response;

namespace ProductService.Application.UseCases.v1.Queries.ProductQueries.GetProducts;

public class GetProductsHandler : IRequestHandler<GetProductsQuery, GetProductsResponse>
{
    private readonly ILogger<GetProductsHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    public GetProductsHandler(ILogger<GetProductsHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<GetProductsResponse> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        const string functionName = $"{nameof(GetProductsHandler)} => ";
        var payload = request.Payload;
        var response = new GetProductsResponse
        {
            Status = ResponseStatusCode.OK.ToInt()
        };

        _logger.LogInformation($"{functionName}");

        try
        {
            var productData = await (from product in _unitOfWork.Product.GetQueryable()
                    select new GetProductData
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        Quantity = product.Quantity,
                    })
                .AsNoTracking()
                .ToListAsPageAsync(payload.PageNumber, payload.MaxPerPage, cancellationToken);

            response.Data = productData.Data;
            response.Paging = productData.Paging;

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GetProductsHandler)} Has error => {ex.Message}");
            response.Status = ResponseStatusCode.InternalServerError.ToInt();
            response.ErrorMessage = "Something went wrong";
            //response.ErrorMessageCode = ""
            return response;
        }
    }
}