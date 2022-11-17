namespace WebhookManager.Domain.Models;

public class WebhookSubscription
{
    public long Id { get; init; }
    public required Uri Url { get; init; }
}
