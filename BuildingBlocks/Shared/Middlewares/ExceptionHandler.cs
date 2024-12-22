using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Shared.Middlewares;

public class ExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService  _problemDetailsService;
    private readonly ILogger<ExceptionHandler> _logger;

    public ExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<ExceptionHandler> logger)
    {
        _problemDetailsService = problemDetailsService;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // catch (ValidationException ex)
        //     httpContext.Response.ContentType = "application/json";
        //     httpContext.Response.StatusCode = ex.ValidationResultModel.StatusCode;
        //     var error = ex.ValidationResultModel.Errors.FirstOrDefault();
        //     if (error != null)
        //     {
        //         var response = new BaseHandler
        //         {
        //             Status = httpContext.Response.StatusCode,
        //             ErrorMessage = error.ErrorMessage,
        //             ErrorMessageCode = error.ErrorMessageCode,
        //             ErrorDetails = ex.ValidationResultModel.Errors
        //         };
        //         await httpContext.Response.WriteAsync(response.ToString());
        //     }
        
        var exceptionMessage = exception.Message;
        _logger.LogError("Error Message: {exceptionMessage}, Time of occurrence {time}", exceptionMessage, DateTime.UtcNow);
        
        var problemDetails = new ProblemDetails
        {
            Status = exception switch
            {
                ArgumentException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            },
            Title = "An error occurred",
            Type = exception.GetType().Name,
            Detail = exception.Message
        };

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            Exception = exception,
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });
    }

    // private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    // {
    //     context.Response.ContentType = "application/json";
    //     context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
    //
    //     await context.Response.WriteAsync
    //     (
    //         new BaseHandler
    //         {
    //             Status = context.Response.StatusCode,
    //             ErrorMessage = message + " - " + exception.Message,
    //             ErrorMessageCode = ResponseErrorMessageCode.ERR_SYS_0001
    //         }.ToString()
    //     );
    // }
}