using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PublishMessageWitDifferentFormatAndType;

public class Helper
{
    public enum MessageFormat
    {
        None,
        Xml,
        Json,
        Binary
    }

    public static MessageFormat GetMessageFormat(string format)
    {
        format = format.ToLower();
        if (format == "1")
        {
            return MessageFormat.Xml;
        }
        else if (format == "2")
        {
            return MessageFormat.Json;
        }
        else if (format == "3")
        {
            return MessageFormat.Binary;
        }
        else
            return MessageFormat.None;
    }

    public static byte[] SerializeMessage(object myMessage, MessageFormat messageFormat)
    {
        if (messageFormat == MessageFormat.Json)
        {
            var jsonString = JsonSerializer.Serialize(myMessage);
            return Encoding.Default.GetBytes(jsonString);
        }
        else if (messageFormat == MessageFormat.Xml)
        {
            var messageStream = new MemoryStream();
            var xmlSerializer = new XmlSerializer(myMessage.GetType());
            xmlSerializer.Serialize(messageStream, myMessage);
            messageStream.Flush();
            messageStream.Seek(0, SeekOrigin.Begin);
            return messageStream.GetBuffer();
        }
        else if (messageFormat == MessageFormat.Binary)
        {
            var messageStream = new MemoryStream();
            var binarySerializer = new BinaryFormatter();
            binarySerializer.Serialize(messageStream, myMessage);
            messageStream.Flush();
            messageStream.Seek(0, SeekOrigin.Begin);
            return messageStream.GetBuffer();
        }
        else
            return null;
    }

    public static string GetContentType(MessageFormat format)
    {
        if (format == MessageFormat.Json)
        {
            return "application/json";
        }
        else if (format == MessageFormat.Xml)
        {
            return "text/xml";
        }
        else if (format == MessageFormat.Binary)
        {
            return "application/octet-stream";
        }
        else
            return string.Empty;
    }

    public static string GetMessageType(object message)
    {
        var assemblyQualifiedName = message.GetType().AssemblyQualifiedName;
        return assemblyQualifiedName ?? "";
    }

    public static object GetMessageObject(string? type)
    {
        if (type == "1")
            return new MyMessage("Message");
        else
            return new MyMessage2("message2", "message2");
    }
}