namespace Shared.Models.Response;

public class ErrorServiceResponse
{
    public bool HasError { get; set; }
    public int Status { get; set; }
    public string ErrorMessageCode { get; set; } = string.Empty;
}