using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Shared.EventBus;
using Shared.RabbitMQ.Internals;

namespace Shared.RabbitMQ
{
    public sealed class RabbitMQBusConfigurer : IBusConfigurer
    {
        private readonly IServiceCollection _services;
        private readonly RabbitMQPersistentConnectionConfiguration _connectionConfig = new();
        private readonly ConnectionFactoryProvider _connectionFactoryProvider = new();
        private readonly RabbitMQDeclarator _declarator = new();
        private readonly RabbitMQEndPoints _endPoints = new();

        public RabbitMQBusConfigurer(IServiceCollection services)
        {
            _services = services;
        }

        public RabbitMQBusConfigurer ConfigureConnection(Action<IConnectionFactory> configure)
        {
            return ConfigureConnection((f, opts) => configure(f));
        }

        public RabbitMQBusConfigurer ConfigureConnection(Action<IConnectionFactory, IRabbitMQPersistentConnectionConfiguration> configure)
        {
            _connectionFactoryProvider.SetConnectionFactory(new ConnectionFactory());
            configure(_connectionFactoryProvider.GetConnectionFactory(), _connectionConfig);
            return this;
        }

        public RabbitMQBusConfigurer Declare(Action<IRabbitMQDeclarator> declare)
        {
            declare(_declarator);
            return this;
        }

        public RabbitMQBusConfigurer ConfigureEndPoint(string exchange, string queue, Action<IRabbitMQEndPoint> configure)
        {
            _declarator.DefaultExchangeDeclare(exchange);
            _declarator.DefaultQueueDeclare(queue);
            configure(_endPoints.RegistedEndPoint(exchange, queue));
            return this;
        }

        public void Complete()
        {
            _services.AddSingleton<IRabbitMQPersistentConnection, RabbitMQPersistentConnection>();
            _services.AddSingleton<IRabbitMQPersistentChannel, RabbitMQPersistentChannel>();
            _services.AddSingleton<IRabbitMQPublisher, RabbitMQPublisher>();
            _services.AddSingleton<IBasicPropertiesProvider>(new SimpleBasicPropertiesProvider(new BasicProperties { DeliveryMode = DeliveryModes.Persistent }));
            _services.AddSingleton<IRabbitMQPersistentConnectionConfiguration>(_connectionConfig);
            _services.AddSingleton<IConnectionFactoryProvider>(_connectionFactoryProvider);
            _services.AddSingleton<IRabbitMQDeclarationCollection>(_declarator);
            _services.AddSingleton<IRabbitMQEndPointCollection>(_endPoints);
            _services.AddSingleton<IEventBusInstance, RabbitMQBusInstance>();
        }
    }
}
