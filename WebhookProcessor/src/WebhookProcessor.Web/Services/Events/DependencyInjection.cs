using Common.Events.Listener.RabbitMq;

namespace WebhookProcessor.Web.Services.Events;

public static class DependencyInjection
{
    public static IServiceCollection AddWebhookListening(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<WebhookEventProcessor>();
        services.AddHostedService<WebhookEventProcessorService>();

        services.Configure<RabbitMqListeningConfiguration>(configuration.GetSection("RabbitMq"));
        return services;
    }
}