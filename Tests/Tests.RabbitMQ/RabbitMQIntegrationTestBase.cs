using f14.MessageBus.RabbitMQ;
using f14.MessageBus.RabbitMQ.Internals;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using Testcontainers.RabbitMq;

namespace Tests.RabbitMQ
{
    public class RabbitMQIntegrationTestBase : IAsyncLifetime
    {
        private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder().Build();

        #region IAsyncLifetime

        public async ValueTask InitializeAsync()
        {
            await _rabbitMqContainer.StartAsync().ConfigureAwait(false);
        }

        public async ValueTask DisposeAsync()
        {
            await _rabbitMqContainer.DisposeAsync();
        }

        #endregion

        #region Utility

        protected static ILogger<T> CreateLogger<T>() => Mock.Of<ILogger<T>>();

        protected IConnectionFactoryProvider CreateConnectionFactoryProvider()
        {
            var providerMock = new Mock<IConnectionFactoryProvider>();
            providerMock.Setup(p => p.GetConnectionFactory()).Returns(new ConnectionFactory()
            {
                Uri = new Uri(_rabbitMqContainer.GetConnectionString())
            });
            return providerMock.Object;
        }

        protected IRabbitMQPersistentConnection CreateConnection(IConnectionFactoryProvider? connectionFactoryProvider = null)
        {
            connectionFactoryProvider ??= CreateConnectionFactoryProvider();
            return new RabbitMQPersistentConnection(CreateLogger<RabbitMQPersistentConnection>(), connectionFactoryProvider, new RabbitMQPersistentConnectionConfiguration());
        }

        #endregion
    }
}
