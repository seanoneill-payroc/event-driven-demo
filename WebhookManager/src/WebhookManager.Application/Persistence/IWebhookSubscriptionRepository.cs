using WebhookManager.Domain.Models;

namespace WebhookManager.Application.Persistence;

public interface IWebhookSubscriptionRepository
{
    IEnumerable<WebhookSubscription> SubscriptionsForTopic(string topic);
}

public class FakeInMemoryWebhookRepository : IWebhookSubscriptionRepository
{

    Dictionary<string, WebhookSubscription> _subscriptions = new Dictionary<string, WebhookSubscription>()
    {
        {
            "", new() { Id = 1, Url = new Uri("http://webhookconsumer/consume/") }
        },
    };

    public IEnumerable<WebhookSubscription> SubscriptionsForTopic(string topic)
    {
        throw new NotImplementedException();
    }
}
