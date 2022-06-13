using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthHealthcheck;

public class HealthHealthcheck : IHealthCheck
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HealthCheckOptions _options;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private HealthCheckResult _cachedResult = default;
    private DateTime _resultExpirationTimestamp;

    public HealthHealthcheck(IHttpClientFactory httpClientFactory, HealthCheckOptions options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (_resultExpirationTimestamp >= DateTime.Now)
            return _cachedResult;
        try
        {
            await _semaphore.WaitAsync(cancellationToken);
            if (_resultExpirationTimestamp >= DateTime.Now)
                return _cachedResult;

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(_options.EndpointUri, cancellationToken);
            _cachedResult = (int)response.StatusCode switch
            {
                >= 200 and <= 299 => HealthCheckResult.Healthy(),
                _ => HealthCheckResult.Unhealthy()
            };
            _resultExpirationTimestamp = DateTime.Now.AddSeconds(60);
        }
        catch (Exception e)
        {
            _cachedResult = HealthCheckResult.Unhealthy(exception: e);
            _resultExpirationTimestamp = DateTime.Now.AddSeconds(60);
        }
        finally
        {
            _semaphore.Release();
        }

        return _cachedResult;
    }
}
