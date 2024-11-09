using Microsoft.AspNetCore.Http;
using Shared.Constants;
using Shared.Handler;
using Shared.Validation;
using System.Net;

namespace Shared.Middlewares;

public class ExceptionHandleMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandleMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (ArgumentNullException ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
        catch (ArgumentException ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
        catch (DivideByZeroException ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
        catch (FileNotFoundException ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
        catch (FormatException ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
        catch (IndexOutOfRangeException ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
        catch (InvalidOperationException ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
        catch (KeyNotFoundException ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
        catch (NotSupportedException ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
        catch (NullReferenceException ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
        catch (OverflowException ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
        catch (OutOfMemoryException ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
        catch (StackOverflowException ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
        catch (TimeoutException ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
        catch (ValidationException ex)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = ex.ValidationResultModel.StatusCode;
            var error = ex.ValidationResultModel.Errors.FirstOrDefault();
            if (error != null)
            {
                var response = new BaseHandler
                {
                    Status = httpContext.Response.StatusCode,
                    ErrorMessage = error.ErrorMessage,
                    ErrorMessageCode = error.ErrorMessageCode,
                    ErrorDetails = ex.ValidationResultModel.Errors
                };
                await httpContext.Response.WriteAsync(response.ToString());
            }
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var message = exception switch
        {
            ArgumentNullException => "Null argument is passed to a method",
            ArgumentOutOfRangeException => "Value of an argument is outside the range of valid values",
            AccessViolationException => "Access violation error",
            ArgumentException => "A non-null argument that is passed to a method is invalid",
            DivideByZeroException => "An integer or decimal value is divide by zero",
            FileNotFoundException => "A physical file does not exist at the specified location",
            FormatException => "The format of an argument is invalid, or when a composite format string is not well formed",
            IndexOutOfRangeException => "An attempt is made to access an element of an array or collection with an index that is outside its bounds",
            InvalidOperationException => "A method call is invalid for the object's current state",
            KeyNotFoundException => "The key specified for accessing an element in a collection does not match any key in the collection",
            NotSupportedException => "An invoked method is not supported, or when there is an attempt to read, seek, or write to a stream that does not support the invoked functionality",
            NullReferenceException => "There is an attempt to dereference a null object reference",
            OverflowException => "An arithmetic, casting, or conversion operation in a checked context results in an overflow",
            OutOfMemoryException => "There is not enough memory to continue the execution of a program",
            StackOverflowException => "The execution stack exceeds the stack size. This class cannot be inherited",
            TimeoutException => "The time allotted for a process or operation has expired",
            _ => exception.Message
        };

        await context.Response.WriteAsync
        (
            new BaseHandler
            {
                Status = context.Response.StatusCode,
                ErrorMessage = message + " - " + exception.Message,
                ErrorMessageCode = ResponseErrorMessageCode.ERR_SYS_0001
            }.ToString()
        );
    }
}