# Core configuration

The core configuration allows you to provide your own implementation for some core parts `f14.MessageBus`.

The main parts you can override:

- [Message serializer](#message-serializer)


### Message serializer

By default, this package uses `System.Text.Json` and `Encoding.UTF8` to serialize and deserialize data.

You may inspect source code here - [JsonTextMessageSerializer.cs](../src/f14.MessageBus/Internals/JsonTextMessageSerializer.cs).

To provide your implementation of the serializer use next:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMessageBus(setup =>
    {
        setup
            .Consume<..., ...>()            
            .UseMessageSerializer<YourMessageSerializer>()
            .UseBus<...>(config =>
            {
                ...
            });
    });

...

public class YourMessageSerializer : IMessageSerializer
{
    public ValueTask<object?> DeserializeAsync(byte[] message, Type messageType, CancellationToken cancellationToken = default)
    {
        // Your deserialization code
    }

    public ValueTask<byte[]> SerializeAsync(object? message, CancellationToken cancellationToken = default)
    {
        // Your serialization code
    }
}
```