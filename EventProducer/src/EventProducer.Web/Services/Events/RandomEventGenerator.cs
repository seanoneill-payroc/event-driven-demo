using Common.Events;
using GenFu;

namespace EventProducer.Web.Services.Events;

public interface IEventGenerator
{
    Task<Event<TPayload>> NewEvent<TPayload>(string eventName, TPayload payload) where TPayload : class;
}

public class EventGenerator : IEventGenerator
{
    public Task<Event<TPayload>> NewEvent<TPayload>(string eventName, TPayload payload)
        where TPayload : class
    {
        var @event = new Event<TPayload>()
        {
            Metadata = GetMetaData(eventName),
            Payload = payload,
        };

        return Task.FromResult(@event);
    }

    private EventMetadata GetMetaData(string eventName) => new EventMetadata(
        EventId: Guid.NewGuid().ToString(),
        EventDate: DateTimeOffset.UtcNow,
        EventName: eventName);
}

public class RandomEventGenerator
{
    private readonly EventGenerator _eventGenerator;
    private readonly Random _random = new Random();

    private readonly List<Func<(string, object)>> _generators = new List<Func<(string, object)>>()
    {
        FakeUserCreated,
        FakeReportGenerated
    };

    public RandomEventGenerator(EventGenerator eventGenerator)
    {
        _eventGenerator = eventGenerator;
    }

    public Task<Event<object>> RandomEvent()
    {
        var selector = _random.Next(_generators.Count);
        var  (eventName, payload) = _generators.ElementAt(selector).Invoke();
        return _eventGenerator.NewEvent(eventName, payload);
    }

    private static Func<(string eventName, object payload)> FakeUserCreated = () =>
    {
        var payload = A.New<UserCreatedPayload>();
        return ("usermanager.user.created", payload);
    };
    private static Func<(string eventName, object payload)> FakeReportGenerated = () =>
    {
        var payload = A.New<ReportGeneratedPayload>();
        return ("reportservice.report.generated", payload);
    };
}

public class ReportGeneratedPayload
{
    public long ReportId { get; init; }
    public string Url => $"https://reportserver/{ReportId}";
}

public class UserCreatedPayload
{
    public long UserId { get; init; }
    public string UserName { get; init; }
    public string Company { get; init; }
}