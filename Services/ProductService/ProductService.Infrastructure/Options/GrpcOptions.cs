namespace ProductService.Infrastructure.Options;

public class GrpcOptions
{
    public const string OptionName = "GrpcSettings";

    public string ProductServiceUrl { get; set; } = string.Empty;
}