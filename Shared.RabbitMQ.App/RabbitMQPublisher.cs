using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Shared.Commons.Options.Polly;
using Shared.EventBus;
using System.Net.Sockets;

namespace Shared.RabbitMQ.App
{
    internal sealed class RabbitMQPublisher : RabbitMQPublisherBase
    {
        private readonly RabbitMQExchangeInfo _exchangeInfo;
        private readonly BasicProperties _basicProperties;
        private readonly RetryPolicyOptions _retryPolicyOptions;

        public RabbitMQPublisher(
            RabbitMQAppOptions options,
            IBasicPropertiesProvider basicPropertiesProvider,
            ILogger<RabbitMQPublisher> logger,
            IRabbitMQPersistentConnection persistentConnection)
            : base(logger, persistentConnection)
        {
            _exchangeInfo = options.CalculatorExchange ?? throw new InvalidOperationException("CalculatorExchange info required.");
            _retryPolicyOptions = options.PublishRetryPolicy;
            _basicProperties = basicPropertiesProvider.GetBasicProperties();
        }

        protected override async Task ConfigureExchangeDeclareAsync(IChannel channel)
        {
            await channel.ExchangeDeclareAsync(exchange: _exchangeInfo.Name, type: _exchangeInfo.Type);
        }

        protected override async Task ExecuteRetryPolicyAsync(IChannel channel, AsyncRetryPolicy policy, string eventName, byte[] message)
        {
            await policy.ExecuteAsync(async () =>
            {
                await channel.BasicPublishAsync(
                    exchange: _exchangeInfo.Name,
                    routingKey: eventName,
                    mandatory: true,
                    basicProperties: _basicProperties,
                    body: message);
            });
        }

        protected override AsyncRetryPolicy GetRetryPolicy(IntegrationEvent @event)
        {
            return Policy
                .Handle<SocketException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetryAsync(_retryPolicyOptions.RetryCount, _retryPolicyOptions.CalculateDelay, (ex, time) =>
                {
                    Logger.LogWarning(ex, "Failed to publish event: {EID} after {Time} sec. ({Error})", @event.Id, $"{time.TotalSeconds:n0}", ex.Message);
                });
        }
    }
}
