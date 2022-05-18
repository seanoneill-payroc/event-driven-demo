using Common.Events.Brokers.RabbitMq;
using MediatR;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using WebhookManager.Domain.Models;

namespace WebhookManager.Web.Services.Events;

public class RabbitMqConfiguration
{
    public string Host { get; set; }
    public int Port { get; set; } = 5672;
    public string Exchange { get; set; } = "events";
    public string Queue { get; set; } = "webhookmanager";
}

public class RabbitMQEventListener : IHostedService, IDisposable
{
    const string ALLEVENTS_ROUTINGKEY = "#"; //listen to all routing keys
    private readonly IOptions<RabbitMqConfiguration> _options;
    private readonly RabbitMqWebhookNotificationEmitter _emitter;
    private IConnection? _connection;
    private IModel? _channel;
    private AsyncEventingBasicConsumer? _consumer;
    private bool _disposedValue;

    public RabbitMQEventListener(IOptions<RabbitMqConfiguration> options, RabbitMqWebhookNotificationEmitter emitter)
    {
        _options = options;
        _emitter = emitter;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory()
        {
            HostName = _options.Value.Host,
            DispatchConsumersAsync = true,
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        var queueName = _options.Value.Queue;

        _channel.QueueDeclare(queueName, durable: true, autoDelete: false, exclusive: false);
        _channel.QueueBind(queueName, _options.Value.Exchange, ALLEVENTS_ROUTINGKEY);

        _consumer = new AsyncEventingBasicConsumer(_channel);
        _consumer.Received += EventReceived;

        _channel.BasicConsume(queueName, autoAck: false, _consumer);

        return Task.CompletedTask;
    }

    private async Task EventReceived(object sender, BasicDeliverEventArgs @event)
    {

        var json = Encoding.UTF8.GetString(@event.Body.Span);
        try
        {

            var priority = @event.BasicProperties.Headers.ContainsKey("x-priority") ? (long)@event.BasicProperties.Headers["x-priority"] : 3L;
            var enteredQueue = @event.BasicProperties.Headers.ContainsKey("x-entered-queue")
                ? DateTimeOffset.Parse( Encoding.UTF8.GetString( @event.BasicProperties.Headers["x-entered-queue"] as byte[]))
                : DateTimeOffset.UtcNow.AddDays(-1);

            Log.ForContext("body", json)
                .Information("[{Application}|{Service}] event {EventName} {Priority} Recieved Time in Queue: {TimeInQueue}ms", "WebhookManager", nameof(RabbitMQEventListener), @event.RoutingKey, priority, (DateTimeOffset.UtcNow - enteredQueue).TotalMilliseconds);

            //TODO consider scoped processor (it's not a consideration, we'll want that here)
            //TODO error trap
            await _emitter.Emit(new Common.Events.Event<WebhookNotification>()
            {
                Metadata = new Common.Events.EventMetadata(
                    EventName: $"webhook.notification.p{priority}",
                    EventDate: DateTimeOffset.UtcNow,
                    EventId: Guid.NewGuid().ToString()
                ),
                Payload = new WebhookNotification()
                {
                    Subscription = new WebhookSubscription
                    {
                        Id = 4,
                        Url = new Uri("https://thisisstaticly.com"),
                    },
                    Event = json,
                }
            });

            _channel?.BasicAck(@event.DeliveryTag, multiple: false);
        }
        catch(Exception e)
        {
            Log.ForContext("body", json)
                .Error(e, "Error processing message");
            
            _channel?.BasicNack(@event.DeliveryTag, multiple: true, requeue: false); //don't requeue until we have some retry theshold or failure detection
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _connection?.Close();
        _channel?.Close();
        return Task.CompletedTask;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _connection?.Close();
                _channel?.Close();
                _channel?.Dispose();
                _connection?.Dispose();
            }
            _connection = null;
            _channel = null;
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

public class WebhookNotification
{
    public WebhookSubscription Subscription { get; set; }
    public string Event { get; set; }
}

public class RabbitMqWebhookNotificationEmitter : RabbitMqEventBroker
{
    public RabbitMqWebhookNotificationEmitter(IWebHostEnvironment env) : base(Options.Create(new RabbitMqEventBrokerConfiguration()
    {
        Exchange = "webhook.notification",
        Host = env.IsDevelopment() ?  "localhost" : "rabbitmq", //TODO haaaaack
    }))
    {
    }
}