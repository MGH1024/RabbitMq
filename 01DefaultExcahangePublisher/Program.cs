using System.Text;
using RabbitMQ.Client;

const string hostName = "localhost";
const string userName = "guest";
const string password = "guest";
const string queueName = "Queue1";
const string exchangeName = "";

Console.WriteLine("Starting RabbitMQ Message Sender");
Console.WriteLine();


var connectionFactory = new ConnectionFactory { HostName = hostName, UserName = userName, Password = password };
var connection = connectionFactory.CreateConnection();
var model = connection.CreateModel();

model.QueueDeclare(queueName, true, false, false, null);
Console.WriteLine("Queue created");

var properties = model.CreateBasicProperties();
properties.Persistent = false;

//Serialize
var messageBuffer = Encoding.Default.GetBytes("this is my message");

//Send message
model.BasicPublish(exchangeName, queueName, properties, messageBuffer);

Console.WriteLine("Message sent");
Console.ReadLine();