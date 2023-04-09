using System.Text;
using RabbitMQ.Client;

const string hostName = "localhost";
const string userName = "guest";
const string password = "guest";
const string queueName = "Queue2";
const string exchangeName = "Exchange2";
const string routingKey = "RoutingKey2";

Console.WriteLine("Starting RabbitMQ Queue Creator");

var connectionFactory = new ConnectionFactory
{
    HostName = hostName,
    UserName = userName,
    Password = password
};

var connection = connectionFactory.CreateConnection();
var model = connection.CreateModel();

model.QueueDeclare(queueName, true, false, false, null);
Console.WriteLine("Queue created");

model.ExchangeDeclare(exchangeName, ExchangeType.Topic);
Console.WriteLine("Exchange created");

model.QueueBind(queueName, exchangeName, routingKey);
Console.WriteLine("Exchange and queue bound");

var properties = model.CreateBasicProperties();
properties.Persistent = false;

//Serialize
var messageBuffer = Encoding.Default.GetBytes("this is my message");


//Send message
model.BasicPublish(exchangeName, routingKey, properties, messageBuffer);
Console.WriteLine("message sent");

Console.ReadLine();