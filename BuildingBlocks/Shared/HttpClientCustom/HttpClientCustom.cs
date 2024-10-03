using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly.Registry;

namespace Shared.HttpClientCustom;

public class HttpClientCustom<T> : IHttpClientCustom<T> 
{
    private readonly HttpClient _httpClient;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<HttpClientCustom<T>> _logger;
    private readonly ResiliencePipelineProvider<string> _resiliencePipelineProvider;
    
    
    public HttpClientCustom
    (
        HttpClient httpClient,
        IHttpClientFactory httpClientFactory,
        ILogger<HttpClientCustom<T>> logger
    )
    {
        _logger = logger;
        _httpClient = httpClient;
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<TResult?> GetAsync<TResult>(string path, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(IHttpClientCustom<T>)}_{typeof(T).Name}-{path}-{nameof(GetAsync)} => ";
        try
        {
            _logger.LogDebug($"{functionName} is called ...");
            
            var response = await _httpClient.GetAsync(path, cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<TResult>(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{functionName} has error: {ex.Message}");
            return default;
        }
    }
    
    public async Task<TResult?> GetAsync<TResult>(string host, string path, CancellationToken cancellationToken)
    {
        var requestUri = $"{host}/{path}";
        var functionName = $"{nameof(HttpClientCustom<T>)} - {nameof(GetAsync)} =>";
        try
        {
            _logger.LogDebug($"{functionName} is called {requestUri}-{typeof(T).Name}");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

            var httpClient = _httpClientFactory.CreateClient(typeof(T).Name);
            httpClient.Timeout = TimeSpan.FromMinutes(5);

            var response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<TResult>(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{functionName} has error: {ex.Message}");
            return default;
        }
    }
    
    public async Task<TResult?> PostAsync<TResult>(string path, string jsonContent, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(IHttpClientCustom<T>)}_{typeof(T).Name}-{path}-{nameof(PostAsync)} => ";
        
        try
        {
            _logger.LogDebug($"{functionName} is called ...");
            
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(path, content, cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<TResult>(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{functionName} has error: {ex.Message}");
            return default;
        }
    }
    
    public async Task<TResult?> PostAsync<TResult>(string host, string path, string jsonContent, CancellationToken cancellationToken)
    {
        var requestUri = $"{host}/{path}";
        var functionName = $"{nameof(HttpClientCustom<T>)} - {nameof(PostAsync)} =>";
        
        try
        {
            _logger.LogDebug($"{functionName} is called {requestUri}-{typeof(T).Name} => JsonContent = {jsonContent}");
            
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);
            httpRequestMessage.Content = content;

            var httpClient = _httpClientFactory.CreateClient(typeof(T).Name);
            httpClient.Timeout = TimeSpan.FromMinutes(5);

            var response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<TResult>(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{functionName} has error: {ex.Message}");
            return default;
        }
    }
}