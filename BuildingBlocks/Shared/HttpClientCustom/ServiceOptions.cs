
namespace Shared.HttpClientCustom;

public class ServiceOptions
{
    public static readonly string OptionName = "Services";
}

public class ServiceConfig
{
    public string ServiceName { get; set; }
    public string Url { get; set; }
    public int HttpClientTimeout { get; set; } = 6;
    public Retry Retry { get; set; } = new();
    public CircuitBreaker CircuitBreaker { get; set; }
}

public class Retry
{
    public int RetryCount { get; set; } = 0;
    public int SleepDurationInSeconds { get; set; } = 1;
}

public class CircuitBreaker
{
    public int AllowConsecutiveErrors { get; set; }
    public int DurationOfBreakInSeconds { get; set; }
}