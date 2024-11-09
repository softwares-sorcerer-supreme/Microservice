namespace Shared.HttpClientCustom;

public interface IHttpClientCustom<T>
{
    Task<TResult?> GetAsync<TResult>(string path, CancellationToken cancellationToken);

    Task<TResult?> GetAsync<TResult>(string host, string path, CancellationToken cancellationToken);

    Task<TResult?> PostAsync<TResult>(string path, string jsonContent, CancellationToken cancellationToken);

    Task<TResult?> PostAsync<TResult>(string host, string path, string jsonContent, CancellationToken cancellationToken);
}