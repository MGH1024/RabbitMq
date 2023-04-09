namespace PublishMessageWitDifferentFormatAndType;

[Serializable]
public class MyMessage2
{
    public MyMessage2()
    {
            
    }
    public MyMessage2(string message,string message2)
    {
        Message = message;
        Message2 = message2;
    }
    public string Message { get; set; }
    public string Message2 { get; set; }
}