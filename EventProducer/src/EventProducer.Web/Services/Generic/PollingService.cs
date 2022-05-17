using Serilog.Context;

namespace EventProducer.Web.Services.Generic;

public interface IBackgroundProcessor
{
    Task Execute(CancellationToken cancellationToken = default);
}
public class PollingConfiguration
{
    public virtual TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(15);
}
public class PollingService<TProcessor> : BackgroundService
    where TProcessor : IBackgroundProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly PollingConfiguration _pollingConfiguration;
    private readonly PeriodicTimer _timer;

    public PollingService(IServiceScopeFactory scopeFactory, PollingConfiguration pollingConfiguration)
    {
        _scopeFactory = scopeFactory;
        _pollingConfiguration = pollingConfiguration;
        _timer = new PeriodicTimer(pollingConfiguration.Interval);
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {
            await _timer.WaitForNextTickAsync(stoppingToken);

            using (LogContext.PushProperty("Service", this))
            using (LogContext.PushProperty("Processor", typeof(TProcessor)))
            {
                using var scope = _scopeFactory.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<TProcessor>();
                Log.Information("{Service} executing {Processor}");
                try
                {
                    await processor.Execute(stoppingToken);
                    Log.Information("{Processor} completed successfully");
                }
                catch (Exception e)
                {
                    Log.Error(e, "Error executing {Processor}");
                }
            }
        }
    }
}
