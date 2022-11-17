using Common.Events.Listener.RabbitMq;
using RabbitMQ.Client.Events;
using ILogger = Serilog.ILogger;

namespace WebhookProcessor.Web.Services.Events;

public class WebhookEventProcessor : IRabbitMqMessageReceviedProcessor
{
    private readonly ILogger _logger;

    public WebhookEventProcessor(ILogger logger)
    {
        _logger = logger.ForContext<WebhookEventProcessor>();
    }
    public Task ProcessMessage(AsyncEventingBasicConsumer? consumer, BasicDeliverEventArgs @event)
    {
        var exchange = @event.Exchange;
        var routingKey = @event.RoutingKey;

        _logger.Information("Message received {Exchange}, {RoutingKey}", exchange, routingKey);

        consumer?.Model.BasicAck(@event.DeliveryTag, multiple: false);

        return Task.CompletedTask;
    }
}