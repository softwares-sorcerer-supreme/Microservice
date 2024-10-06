using Microsoft.AspNetCore.Mvc;
using Shared.Models.Dtos;

namespace Shared.Models.Response;

public class BaseResponse
{
    public int Status { get; set; }
    public string ErrorMessageCode { get; set; } = string.Empty;
}

public static class ResponseHelper
{
    public static ObjectResult ToResponse(int httpStatusCode, string errorMessageCode = "", object data = null)
    {
        return new ObjectResult(new
        {
            status = httpStatusCode,
            errorMessageCode,
            data
        })
        { StatusCode = httpStatusCode };
    }

    public static ObjectResult ToPaginationResponse(int httpStatusCode, string errorMessageCode, object data = null, PagingDto paging = null)
    {
        return new ObjectResult(new
        {
            status = httpStatusCode,
            errorMessageCode,
            data,
            paging
        })
        { StatusCode = httpStatusCode };
    }
}