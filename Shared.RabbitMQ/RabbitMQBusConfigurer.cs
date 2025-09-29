using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Shared.Commons;
using Shared.EventBus;
using Shared.RabbitMQ.Internals;

namespace Shared.RabbitMQ
{
    public sealed class RabbitMQBusConfigurer : IBusConfigurer
    {
        private readonly IServiceCollection _services;
        private readonly RabbitMQPersistentConnectionConfiguration _connectionConfig = new();
        private readonly ConnectionFactoryProvider _connectionFactoryProvider = new();
        private readonly BasicPropertiesProvider _basicPropertiesProvider = new();
        private readonly RabbitMQDeclarator _declarator = new();
        private readonly RabbitMQEndPoints _publisherEndPoints = new();
        private readonly RabbitMQEndPoints _consumerEndPoints = new();

        public RabbitMQBusConfigurer(IServiceCollection services)
        {
            _services = services;
        }

        public RabbitMQBusConfigurer Connection(Action<IConnectionFactory, IRabbitMQPersistentConnectionConfiguration> configure)
        {
            _connectionFactoryProvider.SetConnectionFactory(new ConnectionFactory());
            configure(_connectionFactoryProvider.GetConnectionFactory(), _connectionConfig);
            return this;
        }

        public RabbitMQBusConfigurer BasicProperties(Action<BasicProperties> configure)
        {
            configure(_basicPropertiesProvider.GetBasicProperties());
            return this;
        }

        public RabbitMQBusConfigurer PublishEndPoint(string exchange, string queue, Action<IRabbitMQEndPoint> configure)
        {
            MapEndPoint(_publisherEndPoints, exchange, queue, configure);
            return this;
        }

        public RabbitMQBusConfigurer PublishEndPoint<TMessage>(string exchange, string queue)
        {
            MapEndPoint<TMessage>(_publisherEndPoints, exchange, queue);
            return this;
        }

        public RabbitMQBusConfigurer ConsumeEndPoint(string exchange, string queue, Action<IRabbitMQEndPoint> configure)
        {
            MapEndPoint(_consumerEndPoints, exchange, queue, configure);
            return this;
        }

        public RabbitMQBusConfigurer ConsumeEndPoint<TMessage>(string exchange, string queue)
        {
            MapEndPoint<TMessage>(_consumerEndPoints, exchange, queue);
            return this;
        }

        public void Complete()
        {
            _services.AddSingleton<IRabbitMQPersistentConnection, RabbitMQPersistentConnection>();
            _services.AddSingleton<IBasicPropertiesProvider>(_basicPropertiesProvider);
            _services.AddSingleton<IRabbitMQPersistentConnectionConfiguration>(_connectionConfig);
            _services.AddSingleton<IConnectionFactoryProvider>(_connectionFactoryProvider);
            _services.AddSingleton<IRabbitMQDeclarationCollection>(_declarator);
            _services.AddSingleton<IEventBusInstance>(sp =>
            {
                IRabbitMQPublisher? publisher = _publisherEndPoints.Count > 0 ? ActivatorUtilities.CreateInstance<RabbitMQPublisher>(sp, _publisherEndPoints) : null;
                IRabbitMQConsumerChannel? consumerChannel = _consumerEndPoints.Count > 0 ? ActivatorUtilities.CreateInstance<RabbitMQConsumerChannel>(sp, _consumerEndPoints) : null;

                var args = new List<object>();
                if (publisher.IsNotNull())
                {
                    args.Add(publisher);
                }
                if (consumerChannel.IsNotNull())
                {
                    args.Add(consumerChannel);
                }

                if (args.Count == 0)
                {
                    throw RabbitMQConfigurationException.PublisherAndConsumerIsNull();
                }

                return ActivatorUtilities.CreateInstance<RabbitMQBusInstance>(sp, [.. args]);
            });
        }

        private void MapEndPoint(RabbitMQEndPoints endPoints, string exchange, string queue, Action<IRabbitMQEndPoint> configure)
        {
            _declarator.DefaultExchangeDeclare(exchange);
            _declarator.DefaultQueueDeclare(queue);
            configure(endPoints.RegistedEndPoint(exchange, queue));
        }

        private void MapEndPoint<TMessage>(RabbitMQEndPoints endPoints, string exchange, string queue)
        {
            _declarator.DefaultExchangeDeclare(exchange);
            _declarator.DefaultQueueDeclare(queue);
            endPoints.RegistedEndPoint(exchange, queue).Message<TMessage>();
        }
    }
}
