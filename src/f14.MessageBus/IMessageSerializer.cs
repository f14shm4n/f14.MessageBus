namespace f14.MessageBus
{
    public interface IMessageSerializer
    {
        byte[] Serialize(object? message);
        object? Deserialize(byte[] message, Type messageType);
    }
}
