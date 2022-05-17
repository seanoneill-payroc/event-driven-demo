using Common.Events.Brokers.RabbitMq;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using WebhookManager.Domain.Models;

namespace WebhookManager.Infrastructure.Services;

public class WebhookNotificationQueueConfiguration : RabbitMqEventBrokerConfiguration
{
    public string Host { get; set; }
    public int Port { get; set; } = 5672;
    public string ExchangeName { get; set; } = "webhook.notifications";
    public string ExchangeType => RabbitMQ.Client.ExchangeType.Direct;

}

public class WebhookNotificationQueue 
{
    private readonly WebhookNotificationQueueConfiguration _configuration;
    private IConnection _connection;
    private IModel _channel;


    public WebhookNotificationQueue(IOptions<WebhookNotificationQueueConfiguration> options) //TODO: improve options
    {
        _configuration = options.Value;
    }

    public Task Initialize()
    {
        var factory = new ConnectionFactory()
        {
            HostName = _configuration.Host,
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(_configuration.ExchangeName, _configuration.ExchangeType, durable: true, autoDelete: false);

        return Task.CompletedTask;
    }

    public Task Emit(WebhookSubscription subscription, ReadOnlyMemory<byte> eventPayload)
    {

        var job = new WebhookJob()
        {
            WebhookSubscriptionId = subscription.Id,
            WebhookUri = subscription.Url?.AbsoluteUri ?? throw new ArgumentException("Fully qualified url required", nameof(subscription.Url)),
            Payload = eventPayload,
        };

        var bytes = JsonSerializer.SerializeToUtf8Bytes(job, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        
        var priority = 1;
        _channel.BasicPublish(_configuration.ExchangeName, $"webhook.notification.p{priority}", body: bytes);
        return Task.CompletedTask;
    }
}

public class WebhookJob
{
    public long WebhookSubscriptionId { get; init; }
    public string WebhookUri { get; init; } //this is potentially volatile and should be derived from the subscription id at the consumer
    public ReadOnlyMemory<byte> Payload { get; init; }
}
