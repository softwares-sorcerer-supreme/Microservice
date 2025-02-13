using Newtonsoft.Json;
using Shared.Models.Response;

namespace Shared.Handler
{
    public record BaseHandler : BaseResponse
    {
        public string ErrorMessage { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}