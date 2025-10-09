# f14.MessageBus

### **[STAGE]**: `Development`

This project provides a common infrastructure of a message bus.

|Package|Version|.NET|
|-|-|-|
|f14.MessageBus|`8.0.0.0`|8.0|
|f14.MessageBus.RabbitMQ|`8.0.0.0`|8.0|


| Broker | Support |
|--------|---------|
|RabbitMQ|`yes, in work`|
|Apache Kafka|`in plans`|


## How to use

This project is currently targeted for use with `Asp.Net Core DI container`. 

So, if you are not using Asp.Net Core, you will need to use `IServiceCollection` in your project.

**Content**:

- [Core configuration](docs/extra_configuration.md)

**Rabbit MQ**:

- [Extra configuration](docs/rabbitmq_extra_configuration.md)
- [Publisher](docs/rabbitmq_publisher.md)
- [Consumer](docs/rabbitmq_consumer.md)