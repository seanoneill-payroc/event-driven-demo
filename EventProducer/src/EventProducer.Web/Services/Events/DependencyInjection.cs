using Common.Events;
using Common.Events.Brokers.RabbitMq;

namespace EventProducer.Web.Services.Events;

public static class DependencyInjection
{
    public static IServiceCollection AddEventSystems(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RandomEventEmitterConfiguration>(configuration.GetSection("RandomEventEmitter"));
        services.Configure<RabbitMqEventBrokerConfiguration>(configuration.GetSection("RabbitMq"));

        services.AddScoped<RandomEventEmitterProcessor>();
        services.AddHostedService<PeriodicRandomEventEmitterService>();

        services.AddScoped<EventGenerator>();

        services.AddSingleton<IEventBroker, RabbitMqEventBroker>();
        services.AddSingleton<IEventEmitter, EventEmitter>();

        services.AddScoped<RandomEventGenerator>();

        return services;
    }
}
