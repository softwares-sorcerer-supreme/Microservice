using ProductService.Application.Grpc.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application.Services.GrpcService;

public class ProductService
{
    private readonly ProductProtoService.ProductProtoServiceClient _productProtoServiceClient;

    public ProductService(ProductProtoService.ProductProtoServiceClient productProtoServiceClient)
    {
        _productProtoServiceClient = productProtoServiceClient;
    }

    public async Task<GetProductsByIdsResponse> GetProductsByIds(List<Guid> ids)
    {
        var strIds = ids.Select(x => x.ToString());
        var productRequest = new GetProductsByIdsRequest
        {
            ProductIds = 
            {
                strIds
            }
        };
        
        return await _productProtoServiceClient.GetProductsByIdsAsync(productRequest);
    }

    public async Task<UpdateProductQuantityResponse> UpdateProductQuantity(Guid id, int quantity)
    {
        var productRequest = new UpdateProductQuantityRequest
        {
            Id = id.ToString(),
            Quantity = quantity
        };

        return await _productProtoServiceClient.UpdateProductQuantityAsync(productRequest);
    }
}