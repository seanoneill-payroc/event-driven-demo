using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Serilog;
using System.Text;
using System.Text.Json;

namespace Common.Events.Brokers.RabbitMq;

public class RabbitMqEventBrokerConfiguration
{
    public string Host { get; set; }
    public int Port { get; set; } = 5672;
    public string Exchange { get; set; }
}

public class RabbitMqEventBroker : IEventBroker, IDisposable
{
    private readonly IOptions<RabbitMqEventBrokerConfiguration> _options;
    private readonly Random _random = new Random(); //generate a random property;
    private IConnection? _connection;
    private IModel? _channel;

    private bool _disposedValue;

    public RabbitMqEventBroker(IOptions<RabbitMqEventBrokerConfiguration> options)
    {
        _options = options;
    }

    private Task Connect()
    {
        var factory = new ConnectionFactory()
        {
            HostName = _options.Value.Host,

        };

        _connection = factory.CreateConnection();
        _connection.ConnectionShutdown += connection_ConnectionShutdown;
        _channel = _connection.CreateModel();


        _channel.ExchangeDeclare(
            exchange: _options.Value.Exchange,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false
        );

        return Task.CompletedTask;
    }

    private void connection_ConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        Log.ForContext("Details", e, destructureObjects: true).Warning("Connection shutdown");
        _connection = null;
        _channel = null;
    }

    public async Task Emit<TPayload>(Event<TPayload> @event, CancellationToken cancellationToken = default)
        where TPayload : class
    {
        Log.Information("{Broker} emits {@object}", nameof(RabbitMqEventBroker), @event);

        if (_connection is null)
            await Connect();

        var json = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(json);

        try
        {
            var properties = _channel?.CreateBasicProperties();
            properties.Headers = new Dictionary<string,object>()
            {
                { "x-priority", (long)_random.Next(1,3)  },
                { "x-entered-queue", DateTimeOffset.UtcNow.ToString() },
            };
            _channel?.BasicPublish(
                exchange: Exchange,
                routingKey: @event.Metadata.EventName,
                body: body,
                basicProperties: properties
            );
        }
        catch (Exception e)
        {
            Log.Error(e, "Error publishing event");
        }
    }

    private string Exchange => _options.Value.Exchange;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                Log.Information("Closing rabbitmq connections");
                _channel?.Close();
                _connection?.Close();
                _connection?.Dispose();
                _channel?.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}