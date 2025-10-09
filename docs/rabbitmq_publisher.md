# Publisher setup


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