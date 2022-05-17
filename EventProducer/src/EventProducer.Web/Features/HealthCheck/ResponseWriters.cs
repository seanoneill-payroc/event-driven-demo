using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EventProducer.Web.Features.HealthCheck;
public static class ResponseWriters
{
    static readonly JsonSerializerOptions _jsonOptions;
    static ResponseWriters()
    {
        _jsonOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
        };
        _jsonOptions.Converters.Add(new JsonStringEnumConverter());
    }
    public static async Task JsonReportWriter(HttpContext context, HealthReport report)
    {
        var status = report.Status.ToString();
        var details = report.Entries.ToDictionary(
            e => e.Key,
            e => new
            {
                e.Value.Status,
                e.Value.Description,
                Benchmark = e.Value.Duration
            });

        await context.Response.WriteAsJsonAsync(new //TODO strongly type
        {
            Status = status,
            Details = details,
        }, _jsonOptions);

    }
}
