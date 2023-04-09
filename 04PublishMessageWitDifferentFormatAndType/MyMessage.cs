namespace PublishMessageWitDifferentFormatAndType;

[Serializable]
public class MyMessage
{
    public MyMessage()
    {
            
    }
    public MyMessage(string message)
    {
        Message = message;
    }

    public string Message { get; set; }
}