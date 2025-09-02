using DotNet.Testcontainers.Builders;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using Shared.RabbitMQ;
using Shared.RabbitMQ.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        protected RabbitMQAppOptions CreateOptions()
        {
            return new RabbitMQAppOptions
            {
                ConnectionString = _rabbitMqContainer.GetConnectionString(),
                CalculatorExchange = new Shared.RabbitMQ.RabbitMQExchangeInfo
                {
                    Name = "calExchange",
                    Type = "direct",
                    Queue = "operations"
                },
                ConnectionRetryPolicy = new Shared.RabbitMQ.RabbitMQRetryPolicyOptions
                {
                    RetryCount = 1,
                    RetryDelayInMilliseconds = 500
                }
            };
        }

        protected IConnectionFactoryProvider CreateConnectionFactoryProvider()
        {
            var providerMock = new Mock<IConnectionFactoryProvider>();
            providerMock.Setup(p => p.GetConnectionFactory()).Returns(new ConnectionFactory()
            {
                Uri = new Uri(_rabbitMqContainer.GetConnectionString())
            });
            return providerMock.Object;
        }

        protected IRabbitMQPersistentConnection CreateConnection(RabbitMQAppOptions options, IConnectionFactoryProvider? connectionFactoryProvider = null)
        {
            connectionFactoryProvider ??= CreateConnectionFactoryProvider();
            return new RabbitMQPersistentConnection(CreateLogger<RabbitMQPersistentConnection>(), connectionFactoryProvider, options);
        }   

        #endregion
    }
}
