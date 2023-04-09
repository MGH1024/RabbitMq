using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;
using System.Text;

namespace Subscriber;

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

                _connection = _connectionFactory.CreateConnection(clientProvidedName: "Subscriber Connection");

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
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: "TestExchange", type: "direct", durable: true, autoDelete: false, arguments: null);

            _channel.QueueDeclare(queue: "TestQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);

            //var queueArguments = new Dictionary<string, object>() { { "x-dead-letter-exchange", "DeadLetterExchange" } };

            //_channel.QueueDeclare(queue: "TestQueue", durable: true, exclusive: false, autoDelete: false, arguments: queueArguments);

            _channel.QueueBind(queue: "TestQueue", exchange: "TestExchange", routingKey: "MessageFromPublisher");
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 100, global: false);

            _channel.CallbackException += Channel_CallbackException;

            var eventingBasicConsumer = new EventingBasicConsumer(_channel);

            eventingBasicConsumer.Received += Consumer_Received;

            _channel.BasicConsume(queue: "TestQueue", autoAck: false, consumer: eventingBasicConsumer);

            Console.WriteLine("Channel connected.");
        }
    }

    private void Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        if(IsChannelConnected)
        {
            _channel.Close();
        }

        ConnectService();
    }
    private void Connection_ConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
    {
        if (IsChannelConnected)
        {
            _channel.Close();
        }

        ConnectService();
    }
    private void Connection_CallbackException(object sender, CallbackExceptionEventArgs e)
    {
        if (IsChannelConnected)
        {
            _channel.Close();
        }

        ConnectService();
    }
    private void Channel_CallbackException(object sender, CallbackExceptionEventArgs e)
    {
        ConnectService();
    }
    private void Consumer_Received(object sender, BasicDeliverEventArgs e)
    {
        var messageJson = Encoding.UTF8.GetString(e.Body.ToArray());

        try
        {
            //throw new Exception();

            _channel.BasicAck(e.DeliveryTag, false);

            Console.WriteLine($"Message received => {messageJson}");
        }
        catch 
        {
            _channel.BasicNack(e.DeliveryTag, false , true);

            //_channel.BasicNack(e.DeliveryTag, false , false);

            Console.WriteLine($"Message failed => {messageJson}");
        }
    }
}


