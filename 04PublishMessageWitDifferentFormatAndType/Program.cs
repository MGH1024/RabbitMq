using PublishMessageWitDifferentFormatAndType;
using RabbitMQ.Client;

Console.WriteLine("enter format of message");
Console.WriteLine("1:xml");
Console.WriteLine("2:json");
Console.WriteLine("3:binary");


var format = Console.ReadLine();

Console.WriteLine("enter type of message");
Console.WriteLine("1:MyMessage");
Console.WriteLine("2:MyMessage2");

var type = Console.ReadLine();


const string hostName = "localhost";
const string userName = "guest";
const string password = "guest";
const string queueName = "Queue4";
const string exchangeName = "Exchange4";
const string routingKey = "RoutingKey4";


var connectionFactory = new ConnectionFactory
{
    HostName = hostName,
    UserName = userName,
    Password = password
};

var connection = connectionFactory.CreateConnection();
var model = connection.CreateModel();


if (string.IsNullOrEmpty(format))
{
    Console.WriteLine("Please select message format");
    return;
}

var myMessage = Helper.GetMessageObject(type);

var messageFormat = Helper.GetMessageFormat(format);
var contentType = Helper.GetContentType(messageFormat);
var messageType = Helper.GetMessageType(myMessage);


//Serialize                    
byte[] messageBuffer = Helper.SerializeMessage(myMessage, messageFormat);

//Setup properties
var properties = model.CreateBasicProperties();
properties.Persistent = true;
properties.ContentType = contentType;
properties.Type = messageType;

//declare exchange and queue
model.QueueDeclare(queueName, true, false, false, null);
Console.WriteLine("Queue created");

model.ExchangeDeclare(exchangeName, ExchangeType.Topic);
Console.WriteLine("Exchange created");

model.QueueBind(queueName, exchangeName, routingKey);
Console.WriteLine("Exchange and queue bound");

//Send message
model.BasicPublish(exchangeName, routingKey, properties, messageBuffer);
Console.WriteLine("message sent");

Console.ReadLine();