using Common.Events.Listener.RabbitMq;
using Microsoft.Extensions.Options;
using ILogger = Serilog.ILogger;

namespace WebhookProcessor.Web.Services.Events;

public class WebhookEventProcessorService : RabbitMqEventListenerService<WebhookEventProcessor>
{
    public WebhookEventProcessorService(
        ILogger logger,
        IOptionsMonitor<RabbitMqListeningConfiguration> options,
        IServiceScopeFactory scopeFactory) : base(logger, options, scopeFactory)
    {
    }
}