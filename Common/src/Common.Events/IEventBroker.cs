namespace Common.Events;

public interface IEventBroker
{
    Task Emit<TPayload>(Event<TPayload> @event, CancellationToken cancellationToken = default)
        where TPayload : class;
}
