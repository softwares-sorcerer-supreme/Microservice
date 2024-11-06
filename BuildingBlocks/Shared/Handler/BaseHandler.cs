using Newtonsoft.Json;
using Shared.Models.Response;
using Shared.Validation;

namespace Shared.Handler
{
    public record BaseHandler : BaseResponse
    {
        public string ErrorMessage { get; set; }
        public List<ValidationError> ErrorDetails { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}