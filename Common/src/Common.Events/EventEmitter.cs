﻿using System.Diagnostics;

namespace Common.Events;

public class EventEmitter : IEventEmitter
//[WIP] this needs to work off a queue that has persistence;
{
    private readonly IEnumerable<IEventBroker> _eventBrokers;

    public EventEmitter(IEnumerable<IEventBroker> eventBrokers)
    {
        _eventBrokers = eventBrokers;
    }

    public async Task Emit<TPayload>(Event<TPayload> @event, CancellationToken cancellationToken = default)
        where TPayload : class
    {
        var timer = Stopwatch.StartNew();
        Log.Information("Emitting events to {BrokerCount} broker {@Brokers}", _eventBrokers.Count(), _eventBrokers.Select(x => x.GetType().Name));
        int counter = 0;
        await Parallel.ForEachAsync(
            source: _eventBrokers,
            cancellationToken: cancellationToken,
            body: async (broker, brokerCancellationToken) =>
            {
                var brokerTimer = Stopwatch.StartNew();
                try
                {
                    await broker.Emit(@event, brokerCancellationToken)
                        .ConfigureAwait(false);
                    Log.Information("{Broker} successfully emitted {@Event} in {ElapsedMS}ms", broker.GetType().Name, @event, brokerTimer.ElapsedMilliseconds);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Error emitting to {Broker} in {ElapsedMS}", broker.GetType().Name, brokerTimer.ElapsedMilliseconds);
                }
                finally
                {
                    Interlocked.Increment(ref counter);
                }
            });
        Log.Information("{System} submitted to {SuccessCount} brokers in {ElapsedMS}ms", nameof(EventEmitter), counter, timer.ElapsedMilliseconds);
    }
}

