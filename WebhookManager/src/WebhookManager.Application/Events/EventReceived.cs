using Common.Events;
using MediatR;

namespace WebhookManager.Application.Events;

public class EventReceived : INotification
{
    public string Topic { get; init; }
    public string Body { get; init; }
}

public class EventReceivedHandler : INotificationHandler<EventReceived>
{
    public Task Handle(EventReceived notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
