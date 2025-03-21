using Microsoft.AspNetCore.Mvc;

namespace Shared.Models.Response;

public class ApiResponse
{
    public int Status { get; set; }
    public string ErrorMessageCode { get; set; } = null!;
    
    public ObjectResult ToResponseData()
    {
        object? data = null;
        return new ObjectResult(new
        {
            status = this.Status,
            errorMessageCode = this.ErrorMessageCode,
            data
        })
        {
            StatusCode = this.Status 
        };
    }
}

public class ApiResponse<TData> : ApiResponse
{
    public TData Data { get; set; }
    
    public new ObjectResult ToResponseData()
    {
        return new ObjectResult(new
        {
            status = this.Status,
            errorMessageCode = this.ErrorMessageCode,
            data = this.Data
        })
        {
            StatusCode = this.Status 
        };
    }
}

public class CursorResponse : ApiResponse
{
    public PageInfo PageInfo { get; }
}

public class PageInfo
{
    public PageInfo(bool hasNextPage, bool hasPreviousPage, string startCursor, string endCursor, long? totalCount)
    {
        HasNextPage = hasNextPage;
        HasPreviousPage = hasPreviousPage;
        StartCursor = startCursor;
        EndCursor = endCursor;
        TotalCount = totalCount;
    }

    public bool HasNextPage { get; }
    public bool HasPreviousPage { get; }
    public string StartCursor { get; }
    public string EndCursor { get; }
    public long? TotalCount { get; }
}