using Common.Events;
using EventProducer.Web.Services.Generic;
using Microsoft.Extensions.Options;
using Serilog.Context;

namespace EventProducer.Web.Services.Events;

public class RandomEventEmitterConfiguration : PollingConfiguration
{
    public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(15);
}

public class PeriodicRandomEventEmitterService : PollingService<RandomEventEmitterProcessor>
{
    public PeriodicRandomEventEmitterService(IServiceScopeFactory scopeFactory, IOptions<RandomEventEmitterConfiguration> options) 
        : base(scopeFactory, options.Value)
    {

    }
}

public class RandomEventEmitterProcessor : IBackgroundProcessor
{
    private readonly RandomEventGenerator _eventGenerator;
    private readonly IEventEmitter _emitter;

    public RandomEventEmitterProcessor(RandomEventGenerator eventGenerator, IEventEmitter emitter)
    {
        _eventGenerator = eventGenerator;
        _emitter = emitter;
    }
    public async Task Execute(CancellationToken cancellationToken = default)
    {
        var @event = await _eventGenerator.RandomEvent();

        using (LogContext.PushProperty("EventId", @event.Metadata.EventId))
        using (LogContext.PushProperty("EventName", @event.Metadata.EventName))
        {
            await _emitter.Emit(@event, cancellationToken);
        }
    }
}