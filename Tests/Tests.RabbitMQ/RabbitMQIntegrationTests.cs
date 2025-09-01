using DotNet.Testcontainers.Builders;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using Shared.RabbitMQ.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.RabbitMq;

namespace Tests.RabbitMQ
{
    public class RabbitMQIntegrationTests : IAsyncLifetime
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

        #region Tests

        [Fact]
        [Trait("DockerPlatform", "Linux")]
        public async Task RabbitMQAppPersistentConnection_IsConnected()
        {   
            var opts = CreateOptions();
            var con = new RabbitMQAppPersistentConnection(Mock.Of<ILogger<RabbitMQAppPersistentConnection>>(), opts);
            var r = await con.TryConnectAsync();

            r.Should().BeTrue();
            con.IsConnected.Should().BeTrue();
        }

        #endregion

        #region Utility

        private RabbitMQAppOptions CreateOptions()
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

        #endregion
    }
}
