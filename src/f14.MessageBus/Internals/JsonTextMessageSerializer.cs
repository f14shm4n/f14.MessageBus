using f14.MessageBus;
using System.Text;
using System.Text.Json;

namespace f14.MessageBus.Internals
{
    internal sealed class JsonTextMessageSerializer : IMessageSerializer
    {
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public object? Deserialize(byte[] message, Type messageType)
        {
            var text = Encoding.UTF8.GetString(message);
            return JsonSerializer.Deserialize(text, messageType, _jsonOptions);
        }

        public byte[] Serialize(object? message)
        {
            return JsonSerializer.SerializeToUtf8Bytes(message);
        }
    }
}
