using MediatR;
using WebhookManager.Domain.Models;

namespace WebhookManager.Application.Subscriptions.Queries;

public class SubscribersForTopicQuery : IRequest<IEnumerable<WebhookSubscription>>
{
    public string Topic { get; init; }
}

public class SubscribersForTopicQueryHandler : IRequestHandler<SubscribersForTopicQuery, IEnumerable<WebhookSubscription>>
{
    public Task<IEnumerable<WebhookSubscription>> Handle(SubscribersForTopicQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
