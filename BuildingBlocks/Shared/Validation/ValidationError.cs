namespace Shared.Validation;

public class ValidationError
{
    public string Field { get; set; }
    public string ErrorMessage { get; set; }
    public string ErrorMessageCode { get; set; }
}