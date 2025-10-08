using System.Text;
using System.Text.Json;

namespace f14.MessageBus.Internals
{
    internal sealed class JsonTextMessageSerializer : IMessageSerializer
    {
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public ValueTask<object?> DeserializeAsync(byte[] message, Type messageType, CancellationToken cancellationToken = default)
        {
            var text = Encoding.UTF8.GetString(message);
            return ValueTask.FromResult(JsonSerializer.Deserialize(text, messageType, _jsonOptions));
        }

        public ValueTask<byte[]> SerializeAsync(object? message, CancellationToken cancellationToken = default)
        {
            return ValueTask.FromResult(JsonSerializer.SerializeToUtf8Bytes(message));
        }
    }
}
