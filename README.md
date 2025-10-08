# f14.MessageBus

This project provides a common infrastructure for implementing a message bus.

Target platform: `.NET 8`

Supports porviders:

- RabbitMQ

## How to use

This project is currently targeted for use with `Asp.Net Core`.

1. Add f14.MessageBus to DI container and configure it.

### Publisher

*Program.cs**

```csharp
...
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMessageBus(setup =>
    {
        setup
            .UseBus<RabbitMQBusConfigurer>(config =>
            {
                config                    
                    .Connection((cf, cc) =>
                    {        
                        // Configure connection to RabbitMQ
                        cf.Uri = new Uri("amqp://guest:guest@localhost:5672/");
                    })
                    // Detailed configuration
                    .PublishEndPoint(c =>
                    {
                        c
                        .Exchange("your_exchange_name", ExchangeType.Direct)
                        .Queue("your_queue_name")
                        .EndPoint(ep => 
                        {
                            endpoint.Message<YourMessage1>();
                            endpoint.Message<YourMessage2>();
                            ...
                            endpoint.Message<YourMessageN>();
                        });
                    })
                    // Or
                    .PublishEndPoint("your_exchange_name", "your_queue_name", endpoint =>
                    {
                        endpoint.Message<YourMessage1>();
                        endpoint.Message<YourMessage2>();
                        ...
                        endpoint.Message<YourMessageN>();
                    });
            });
    });
...
```

### Consumer

*Program.cs**

```csharp
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
```
 
*SpecificMessage.cs*

```csharp
public class SpecificMessage
{
    ...
}
```

*SpecificMessageConsumer.cs*

```csharp

public class SpecificMessageConsumer : IConsumer<SpecificMessage>
{
        public Task ConsumeAsync(SpecificMessage message, CancellationToken token = default)
        {
            // Process your message
            return Task.CompletedTask;
        }
}
```