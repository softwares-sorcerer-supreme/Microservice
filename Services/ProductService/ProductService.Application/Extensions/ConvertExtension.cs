using ProductService.Application.Grpc.Protos;
using ProductService.Application.Models.Response.Products;
using System.Globalization;

namespace ProductService.Application.Extensions;

public static class ConvertExtension
{
    public static ProductModel ToProductModel(this ProductDataResponse product)
    {
        return new ProductModel
        {
            Id = product.Id.ToString(),
            Name = product.Name,
            Price = product.Price.ToString(CultureInfo.InvariantCulture),
            Quantity = product.Quantity
        };
    }
}