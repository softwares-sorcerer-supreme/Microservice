using Ocelot.LoadBalancer.LoadBalancers;
using Ocelot.Responses;
using Ocelot.Values;

namespace ApiGateway.OcelotCustomFeature.CustomLoadBalancers;

public class CustomLoadBalancer : ILoadBalancer
{
    private readonly Func<Task<List<Service>>> _services;
    private readonly object _lock = new();
    private int _last;

    public CustomLoadBalancer(Func<Task<List<Service>>> services)
    {
        _services = services;
    }

    public async Task<Response<ServiceHostAndPort>> Lease(HttpContext httpContext)
    {
        var services = await _services();
        lock (_lock)
        {
            if (_last >= services.Count)
                _last = 0;

            var next = services[_last++];
            return new OkResponse<ServiceHostAndPort>(next.HostAndPort);
        }
    }

    public void Release(ServiceHostAndPort hostAndPort)
    { }
}