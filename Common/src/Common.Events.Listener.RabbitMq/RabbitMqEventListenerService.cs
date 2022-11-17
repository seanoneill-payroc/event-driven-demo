using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using Serilog.Context;

namespace Common.Events.Listener.RabbitMq
{
    public class RabbitMqListeningConfiguration
    {
        public string? Host { get; set; }
        public int Port { get; set; } = 5672;
        public required string? QueueName { get; set; }
        public string? ExchangeName { get; set; }
        public required string RoutingKey { get; init; }
    }

    public interface IRabbitMqMessageReceviedProcessor
    {
        Task ProcessMessage(AsyncEventingBasicConsumer? consumer, BasicDeliverEventArgs @event);
    }

    public class RabbitMqEventListenerService<TProcessor> : IHostedService //TODO IDisposable
        where TProcessor : IRabbitMqMessageReceviedProcessor
    {
        private readonly ILogger _logger;
        private RabbitMqListeningConfiguration _options;
        private readonly IServiceScopeFactory _scopeFactory;
        private IConnection? _connection;
        private IModel? _channel;
        private AsyncEventingBasicConsumer? _consumer;

        public RabbitMqEventListenerService(
            ILogger logger,
            IOptionsMonitor<RabbitMqListeningConfiguration> options,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger.ForContext<RabbitMqEventListenerService<TProcessor>>();
            _options = options.CurrentValue;
            options.OnChange((newValue) =>
            {
                 _options = newValue;
                 _logger.Information("Updated listener configuration, reinitializing");
                 Initialize();
            });
            _scopeFactory = scopeFactory;
        }

        private Task Initialize()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _options.Host,
                DispatchConsumersAsync = true,
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            var queueName = _options.QueueName;

            _channel.QueueDeclare(queueName, durable: true, autoDelete: false, exclusive: false);
            _channel.QueueBind(queueName, _options.QueueName, _options.RoutingKey);

            _logger.Information("### Listening to events for {RoutingKey}", _options.RoutingKey);

            _consumer = new(_channel);
            _consumer.Received += EventReceived;

            _channel.BasicConsume(queueName, autoAck: false, _consumer);

            return Task.CompletedTask;
        }

        private async Task EventReceived(object sender, BasicDeliverEventArgs @event)
        {
            using var scope = _scopeFactory.CreateScope();
            var processor = scope.ServiceProvider.GetRequiredService<TProcessor>();
            using var lc = LogContext.PushProperty("EventProcessorId", Guid.NewGuid());
            var log = _logger.ForContext<TProcessor>();
            try
            {
                await processor.ProcessMessage(sender as AsyncEventingBasicConsumer, @event);
                log.Information("Processed event {Exchange} {DeliveryTag}", @event.Exchange, @event.DeliveryTag);
            }
            catch(Exception e)
            {
                log.Error(e, "Error processing event {Exchange} {DeliveryTag}", @event.Exchange, @event.DeliveryTag);
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Initialize();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _connection?.Close();
            _channel?.Close();
            return Task.CompletedTask;
        }
    }
}
