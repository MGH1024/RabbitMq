### Default Exchange Publisher
In this program exchange name is an empty string.so when a message publish with no named exchange RabbitMQ use default exchange to binding queue.
you can find this concept in this repository [Default Exchange Publisher](https://github.com/MGH1024/RabbitMq/blob/master/DefaultExcahange/Program.cs).<br/>
also a queue declared and with a publish command a simple message sent to default exchange and then queue.

### Topic Exchange Publisher
In this program at first a Queue created. then an Exchange created.after these creation Queue and Exchange binded toghether and message send with routing key <br/>
you can find this concept in this repository [Topic Exchange Publisher With Routing Key](https://github.com/MGH1024/RabbitMq/blob/master/ExchangeAndQueue/Program.cs).<br/>

### Publisher with Different format
In this project you can send message with different format
[Publisher with Different format](https://github.com/MGH1024/RabbitMq/blob/master/03PublishMessageWithDifferentFormat/Program.cs).<br/>

### Publisher with Different format
In this project you can send message with different format and type
[Publisher with Different format and Type](https://github.com/MGH1024/RabbitMq/blob/master/04PublishMessageWitDifferentFormatAndType/Program.cs).<br/>
