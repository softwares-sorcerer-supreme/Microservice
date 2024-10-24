﻿using Shared.Models.Dtos;
using Shared.Models.Response;

namespace ProductService.Application.Models.Response.Products;

public class GetProductsResponse : BaseResponse
{
    public PagingDto Paging { get; set; }
    public List<ProductDataResponse> Data { get; set; }
}
