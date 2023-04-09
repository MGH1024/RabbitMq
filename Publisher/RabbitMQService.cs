using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Publisher;

internal class RabbitMQService : IRabbitMQService
{
    private Policy _connectionPolicy;
    private ConnectionFactory _connectionFactory;
    private IConnection _connection;
    private IModel _channel;
    private bool _isDisposed;

    public RabbitMQService()
    {
        CreateConnectionPolicy();
        CreateConnectionFactory();
        ConnectService();
    }

    private bool IsServiceConnected => _connection is not null && _connection.IsOpen;
    private bool IsChannelConnected => _channel is not null && _channel.IsOpen;

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _channel?.Dispose();
            _connection?.Dispose();

            _isDisposed = true;
        }
    }
    public void Publish(MessageDto message)
    {
        ConnectService();

        var basicProperties = _channel.CreateBasicProperties();
        var messageJson = JsonConvert.SerializeObject(message.Body);
        var messageByte = Encoding.UTF8.GetBytes(messageJson);

        _channel.BasicPublish(exchange: "TestExchange", routingKey: message.RoutingKey, basicProperties: basicProperties, body: messageByte);

        Console.WriteLine($"Message sent => {messageJson}");
    }
    public void Publish(List<MessageDto> messages)
    {
        ConnectService();

        var basicProperties = _channel.CreateBasicProperties();
        var basicPublishBatch = _channel.CreateBasicPublishBatch();

        foreach (var message in messages)
        {
            var messageJson = JsonConvert.SerializeObject(message.Body);
            var messageByte = Encoding.UTF8.GetBytes(messageJson).AsMemory();

            basicPublishBatch.Add("TestExchange", message.RoutingKey, true, basicProperties, messageByte);
        }

        basicPublishBatch.Publish();
    }

    private void CreateConnectionPolicy()
    {
        _connectionPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetry(
                retryCount: 1000,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(5),
                onRetry: (exception, time) =>
                {
                    Console.WriteLine(exception.Message);
                    Console.WriteLine($"Retry after {time}");
                });
    }
    private void CreateConnectionFactory()
    {
        _connectionFactory = new ConnectionFactory()
        {
            UserName = "guest",
            Password = "guest",
            VirtualHost = "/",
            HostName = "localhost",
            Port = 5672
        };

        Console.WriteLine("Connection factory created.");
    }
    private void ConnectService()
    {
        _connectionPolicy.Execute(() =>
        {
            if (!_isDisposed && !IsServiceConnected)
            {
                _connection?.Dispose();

                _connection = _connectionFactory.CreateConnection(clientProvidedName: "Publisher Connection");

                _connection.CallbackException += Connection_CallbackException;
                _connection.ConnectionBlocked += Connection_ConnectionBlocked;
                _connection.ConnectionShutdown += Connection_ConnectionShutdown;

                Console.WriteLine("Connection created.");
            }

            ConnectChannel();
        });
    }
    private void ConnectChannel()
    {
        if (!_isDisposed && !IsChannelConnected)
        {
            _channel?.Dispose();

            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: "TestExchange", type: "direct", durable: true, autoDelete: false, arguments: null);
            _channel.CallbackException += Channel_CallbackException;

            Console.WriteLine("Channel connected.");
        }
    }

    private void Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        ConnectService();
    }
    private void Connection_ConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
    {
        ConnectService();
    }
    private void Connection_CallbackException(object sender, CallbackExceptionEventArgs e)
    {
        ConnectService();
    }
    private void Channel_CallbackException(object sender, CallbackExceptionEventArgs e)
    {
        ConnectService();
    }

}
