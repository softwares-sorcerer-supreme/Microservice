using Newtonsoft.Json;
using Shared.Validation;

namespace ProductService.Application.Handler
{
    public class ErrorHandler
    {
        public int status { get; set; }
        public string statusText { get; set; }
        public string errorMessage { get; set; }
        public string errorMessageCode { get; set; }
        public List<ValidationError> errors { get; set; }
        public object data { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}