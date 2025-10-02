using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

        public RabbitMQBusConfigurer PublishEndPoint(Action<IRabbitMQExchangeConfigurer> configure)
        {
            configure(new RabbitMQExchangeConfigurer(_declarator, _publisherEndPoints));
            return this;
        }

        public RabbitMQBusConfigurer ConsumeEndPoint(Action<IRabbitMQExchangeConfigurer> configure)
        {
            configure(new RabbitMQExchangeConfigurer(_declarator, _consumerEndPoints));
            return this;
        }

        public void Complete()
        {
            _services.AddSingleton<IRabbitMQPersistentConnection, RabbitMQPersistentConnection>();
            _services.AddSingleton<IBasicPropertiesProvider>(_basicPropertiesProvider);
            _services.AddSingleton<IRabbitMQPersistentConnectionConfiguration>(_connectionConfig);
            _services.AddSingleton<IConnectionFactoryProvider>(_connectionFactoryProvider);
            _services.AddSingleton<IRabbitMQDeclarationCollection>(_declarator);
            _services.AddSingleton<IAsyncBasicConsumerFactory, DefaultAsyncEventingBasicConsumerFactory>();
            _services.AddSingleton<IEventBusInstance>(sp =>
            {
                IRabbitMQPublisher? publisher = _publisherEndPoints.Count > 0 ? ActivatorUtilities.CreateInstance<RabbitMQPublisher>(sp, _publisherEndPoints) : null;
                IRabbitMQConsumerChannel? consumerChannel = _consumerEndPoints.Count > 0 ? ActivatorUtilities.CreateInstance<RabbitMQConsumerChannel>(sp, _consumerEndPoints) : null;

                var args = new List<object>();
                if (publisher is not null)
                {
                    args.Add(publisher);
                }
                if (consumerChannel is not null)
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

        internal void ReplaceService(ServiceDescriptor serviceDescriptor) => _services.Replace(serviceDescriptor);
    }
}
