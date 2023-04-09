namespace Publisher;

internal class MessageDto
{
    public string RoutingKey { get; set; }
    public object Body { get; set; }
}
