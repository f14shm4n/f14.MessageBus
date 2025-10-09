# RabbitMQ extra configuration

The RabbitMQ configuration allows you to provide your own implementation for some core parts `f14.MessageBus.RabbitMQ`.

The main parts you can override:

- [Basic properties](#basic-properties)
- [Error resolver](#error-resolver)


### Basic properties

By default, `RabbitMQ.Client.BasicProperties` is configured with a single property `DeliveryMode = DeliveryModes.Persistent`

You can configure basic RabbitMQ properties using the following method:

```csharp
builder.Services
    .AddMessageBus(setup =>
    {
        setup                      
            .UseBus<RabbitMQBusConfigurer>(config =>
            {
                config
                    .BasicProperties(bp =>
                    {
                        // Configure basic properties here
                    })
            });
    });
```

### Error resolver

You can handle some errors that occur in the RabbitMQ worker process.

To handle errors, you need to override some methods using the following approach:

```csharp
public class YourRabbitMQErrorResolver : RabbitMQErrorResolver
{
    // Override this method to handle errors that occur while processing the message.
    // For example you may need this method to resend the message to dead-letter queue
    public override Task<ConsumerResolveAction> ResolveProcessingErrorAsync(string routingKey, ReadOnlyMemory<byte> body, Exception exception, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(ConsumerResolveAction.Ack);
    }
}
```