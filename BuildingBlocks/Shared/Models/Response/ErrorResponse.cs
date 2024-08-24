using Microsoft.AspNetCore.Mvc;
using Shared.Models.Dtos;

namespace Shared.Models.Response;

public class ErrorResponse
{
    public int Status { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public string ErrorMessageCode { get; set; } = string.Empty;
}

public static class ResponseHelper
{
    public static ObjectResult ToResponse(int httpStatusCode, string errorMessage = "", string errorMessageCode = "", object data = null)
    {
        return new ObjectResult(new
        {
            status = httpStatusCode,
            statusText = errorMessage,
            errorMessage,
            errorMessageCode,
            data
        })
        { StatusCode = httpStatusCode };
    }

    public static ObjectResult ToPaginationResponse(int httpStatusCode, string errorMessage, string errorMessageCode, object data = null, PagingDto paging = null)
    {
        return new ObjectResult(new
        {
            status = httpStatusCode,
            statusText = errorMessage,
            errorMessage,
            errorMessageCode,
            data,
            paging
        })
        { StatusCode = httpStatusCode };
    }
}