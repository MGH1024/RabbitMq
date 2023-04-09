using Publisher;

Console.WriteLine("Publisher starts...");

var rabbitMQService = new RabbitMQService();

while (true)
{
    try
    {
        var message = new MessageDto()
        {
            RoutingKey = "MessageFromPublisher",
            Body = new
            {
                Content = Guid.NewGuid()
            }
        };

        rabbitMQService.Publish(message);
    }
    catch { }

    Thread.Sleep(1000);
}

//var messages = new List<MessageDto>();

//while (true)
//{
//    try
//    {
//        for (var i = 0; i < 1000; i++)
//        {
//            var message = new MessageDto()
//            {
//                RoutingKey = "MessageFromPublisher",
//                Body = new
//                {
//                    Content = Guid.NewGuid()
//                }
//            };

//            messages.Add(message);
//        }

//        rabbitMQService.Publish(messages);
//    }
//    catch { }

//    Thread.Sleep(5000);
//}

//Console.ReadLine();
