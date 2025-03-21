using Shared.Models.Response;

namespace ProductService.Application.Models.Response.Products;

public class GetProductsByIdsResponse : ApiResponse<List<ProductDataResponse>>;