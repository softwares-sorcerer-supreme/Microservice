using AutoMapper;
using ProductService.Application.Models.Request.Products;
using ProductService.Application.Models.Response.Products;
using ProductService.Domain.Entities;

namespace ProductService.Application.Mappers;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<CreateProductRequest, Product>();
        CreateMap<Product, GetProductsResponse>();
    }
}