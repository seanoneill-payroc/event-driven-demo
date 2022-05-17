namespace Common.Events;

public class Event<TPayload>
    where TPayload : class
{
    public EventMetadata Metadata { get; init; }
    public TPayload Payload { get; init; }
}

public record EventMetadata(string EventId, DateTimeOffset EventDate, string EventName);
