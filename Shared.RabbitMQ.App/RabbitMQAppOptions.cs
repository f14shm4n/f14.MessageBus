using Shared.Commons.Options.Polly;

namespace Shared.RabbitMQ.App
{
    public sealed class RabbitMQAppOptions
    {
        public string ConnectionString { get; set; } = string.Empty;
        public RetryPolicyOptions ConnectionRetryPolicy { get; set; } = new RetryPolicyOptions();
        public RabbitMQExchangeOptions CalculatorExchange { get; set; } = new RabbitMQExchangeOptions();
        public RetryPolicyOptions PublishRetryPolicy { get; set; } = new RetryPolicyOptions();
    }
}
