using System.Net;
using System.Text.Json;

namespace Shared.Validation;

public class ValidationResultModel
{
    public int StatusCode { get; set; } = (int)HttpStatusCode.BadRequest;
    public List<ValidationError> Errors { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}