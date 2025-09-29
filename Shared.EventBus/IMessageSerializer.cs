namespace Shared.EventBus
{
    public interface IMessageSerializer
    {
        byte[] Serialize(object? message);
        object? Deserialize(byte[] message, Type messageType);
    }
}
