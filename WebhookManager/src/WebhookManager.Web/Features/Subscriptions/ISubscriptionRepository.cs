using WebhookManager.Web.Features.Subscriptions.Models;

namespace WebhookManager.Web.Features.Subscriptions;

public interface IWebhookSubscriptionRepository
{
    IAsyncEnumerable<WebhookSubscription> SubscribersFoTopic(string eventName, CancellationToken cancellationToken = default);
}

public class WebhookSubscriptionRepository : IWebhookSubscriptionRepository
{
    public IAsyncEnumerable<WebhookSubscription> SubscribersFoTopic(string eventName, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}