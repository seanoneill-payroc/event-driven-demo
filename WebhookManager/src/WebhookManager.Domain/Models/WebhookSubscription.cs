namespace WebhookManager.Domain.Models;

public class WebhookSubscription
{
    public long Id { get; set; }
    public Uri? Url { get; set; }
}
