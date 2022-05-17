using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net;

public class SeqHealthCheck : IHealthCheck
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IHostEnvironment _environment;

    public SeqHealthCheck(IHttpClientFactory clientFactory, IHostEnvironment environment)
    {
        _clientFactory = clientFactory;
        _environment = environment;
    }
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var seq_url = _environment.IsProduction() ? "http://seq:5341" : "http://localhost:5341"; //TODO

        var client = _clientFactory.CreateClient();
        var response = await client.GetAsync(seq_url);
        return response.StatusCode switch
        {
            HttpStatusCode.OK or HttpStatusCode.Forbidden => HealthCheckResult.Healthy(),
            _ => HealthCheckResult.Unhealthy(response.StatusCode.ToString())
        };
    }
}