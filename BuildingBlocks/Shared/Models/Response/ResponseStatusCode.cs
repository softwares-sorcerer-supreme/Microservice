namespace Shared.Models.Response;

/* Reference: System.Net.HttpStatusCode */

public enum ResponseStatusCode
{
    // Successful 2xx
    OK = 200,
    Created = 201,
    NoContent = 204,

    // Client Error 4xx
    BadRequest = 400,
    Unauthorized = 401,
    Forbidden = 403,
    NotFound = 404,
    MethodNotAllowed = 405,
    RequestTimeout = 408,
    UnsupportedMediaType = 415,

    // Server Error 5xx
    InternalServerError = 500,
    BadGateway = 502,
    ServiceUnavailable = 503,
    GatewayTimeout = 504,
    HttpVersionNotSupported = 505
}