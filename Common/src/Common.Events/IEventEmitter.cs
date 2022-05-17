namespace Common.Events;

public interface IEventEmitter
{
    Task Emit<TPayload>(Event<TPayload> @event, CancellationToken cancellationToken = default) where TPayload : class;
}

