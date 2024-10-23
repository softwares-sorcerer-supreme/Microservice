
namespace Shared.HttpClientCustom;

public class ServiceOptions
{
    public const string OptionName = "Services";
    
    public ProductServiceClient ProductService { get; set; }
    public OrderServiceClient OrderService { get; set; }
    public CartServiceClient CartService { get; set; }
}

public class ProductServiceClient : ResilienceConfig;
public class OrderServiceClient : ResilienceConfig;
public class CartServiceClient : ResilienceConfig;

public class ResilienceConfig
{
    public string ServiceName { get; set; }
    public string Url { get; set; }
    public int HttpClientTimeout { get; set; } = 5;
    public bool IsEnableRetry { get; set; }
    public bool IsEnableCircuitBreaker { get; set; }
    public bool IsEnableRateLimit { get; set; }
    public RetryOptions Retry { get; set; } = new();
    public CircuitBreakerOptions CircuitBreaker { get; set; } = new();
    public RateLimitOptions RateLimit { get; set; } = new();
}

public class RetryOptions
{
    public int MaxRetryAttempts { get; set; } = 3;
    public double DelayDurationInSeconds { get; set; } = 2;
}

public class CircuitBreakerOptions
{
    public double FailureRatio { get; set; } = 0.5;
    public double SamplingDurationInSeconds { get; set; } = 10;
    public int MinimumThroughput { get; set; } = 10;
    public double BreakDurationInSeconds { get; set; } = 30;
}

public class RateLimitOptions
{
    public int PermitLimit { get; set; } = 1000;
    public int QueueLimit { get; set; }
}