### Consumer

```csharp
// Program.cs
...
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMessageBus(setup =>
    {
        setup
            .Consume<SpecificMessage, SpecificMessageConsumer>() // Register message and consumer
            .UseBus<RabbitMQBusConfigurer>(config =>
            {
                config
                    .Connection((cf, cc) =>
                    {
                        // Configure connection to RabbitMQ
                        cf.Uri = new Uri("amqp://guest:guest@localhost:5672/");
                    })
                    // Detailed configuration
                    .ConsumeEndPoint(c =>
                    {
                        c
                        .Exchange("your_exchange_name", ExchangeType.Direct)
                        .Queue("your_queue_name")
                        .EndPoint(ep => 
                        {
                            endpoint.Message<SpecificMessage>();
                        });
                    })
                    // Or 
                    .ConsumeEndPoint("your_exchange_name", "your_queue_name", endpoint =>
                    {
                        endpoint.Message<SpecificMessage>();
                    });
                    // Or 
                    .ConsumeEndPoint<SpecificMessage>("your_exchange_name", "your_queue_name");
            });
    });
...

// SpecificMessage.cs
public class SpecificMessage
{
    ...
}
...

// SpecificMessageConsumer.cs
public class SpecificMessageConsumer : IConsumer<SpecificMessage>
{
        public Task ConsumeAsync(SpecificMessage message, CancellationToken token = default)
        {
            // Process your message
            return Task.CompletedTask;
        }
}
```