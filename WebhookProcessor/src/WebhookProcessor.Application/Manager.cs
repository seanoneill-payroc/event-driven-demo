using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using Serilog.Context;
using System.Text;
using System.Text.Json;

namespace WebhookProcessor.Application;

public class RabbitMqConfiguration
{
    public string Host { get; set; }
    public string ConnectionName { get; set; }
    public int Port { get; set; } = 5672;
    public string ExchangeName { get; set; }
    public string QueueName { get; set; }
    public string Priority { get; set; } = "*";
}

public class Manager
{
    private readonly RabbitMqConfiguration _configuration;
    private CancellationTokenSource _tokenSource;
    private IConnection _connection;
    private IModel _channel;
    private QueueDeclareOk _queue;
    private AsyncEventingBasicConsumer _consumer;

    public Manager(IOptions<RabbitMqConfiguration> options) //TODO naming 
    {
        _configuration = options.Value;
    }

    private Task Initialize()
    {
        var factory = new ConnectionFactory()
        {
            HostName = _configuration.Host,
            DispatchConsumersAsync = true,
            ClientProvidedName = _configuration.ConnectionName,
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _queue = _channel.QueueDeclare();
        _channel.QueueBind(_queue.QueueName, _configuration.ExchangeName, $"webhook.notification.*");
        
        _consumer = new AsyncEventingBasicConsumer(_channel);
        _consumer.Received += Consumer_Received;

        return Task.CompletedTask;
    }

    public async Task BeginListening(CancellationToken stoppingToken)
    {
        await Initialize();
        _channel.BasicConsume(_queue.QueueName, autoAck: false, _consumer);
        await Task.Delay(TimeSpan.MaxValue, stoppingToken);
    }

    private Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
    {
        var json = Encoding.UTF8.GetString(@event.Body.Span);
        var obj = JsonSerializer.Deserialize<dynamic>(json);

        using (LogContext.PushProperty("Uri", obj.Uri))
        {
            Log.Information("Processing Webhook Notification");
        }
        return Task.CompletedTask;
    }
}
