using Shared.Commons.Options.Polly;

namespace Shared.RabbitMQ.App
{
    public sealed class RabbitMQAppOptions : RabbitMQOptions
    {
        public RabbitMQExchangeInfo? CalculatorExchange { get; set; }
        public RetryPolicyOptions PublishRetryPolicy { get; set; } = new RetryPolicyOptions();
    }
}
