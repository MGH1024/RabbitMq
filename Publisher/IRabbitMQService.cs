namespace Publisher;

internal interface IRabbitMQService : IDisposable
{
    void Publish(MessageDto message);
    void Publish(List<MessageDto> messages);
}