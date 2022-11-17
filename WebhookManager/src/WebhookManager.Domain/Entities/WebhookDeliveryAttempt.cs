namespace WebhookManager.Domain.Entities;

public class WebhookDeliveryAttemptEntity
{
    public long Id { get; set; }
    public long WebhookSubscriptionId { get; set; }
    public required string Topic { get; set; }
    public DateTimeOffset Created { get; set; }
    public int ResponseHttpStatusCode { get; set; }
    public string? ResponseBody { get; set; }
}
