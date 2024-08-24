namespace Shared.Validation;

public class ValidationError
{
    public string Field { get; set; }
    public string ErrorMessage { get; set; }
    public string ErrorMessageCode { get; set; }

    public ValidationError()
    {
    }

    public ValidationError(string field, string errorMessage, string errorMessageCode)
    {
        Field = (field != string.Empty ? field : null) ?? string.Empty;
        ErrorMessage = errorMessage;
        ErrorMessageCode = errorMessageCode;
    }
}