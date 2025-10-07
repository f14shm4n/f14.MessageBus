using f14.RetryPolly;

namespace Shared.RabbitMQ.App
{
    public sealed class RabbitMQAppOptions
    {
        public string ConnectionString { get; set; } = string.Empty;
        public RetryPolicyInfo ConnectionRetryPolicy { get; set; } = new RetryPolicyInfo();
        public RabbitMQExchangeOptions CalculatorExchange { get; set; } = new RabbitMQExchangeOptions();
        public RetryPolicyInfo PublishRetryPolicy { get; set; } = new RetryPolicyInfo();
    }
}
