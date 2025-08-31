namespace Shared.RabbitMQ.App
{
    public sealed class RabbitMQAppOptions : RabbitMQOptions
    {
        public RabbitMQExchangeInfo? CalculatorExchange { get; set; }
        public RabbitMQRetryPolicyOptions PublishRetryPolicy { get; set; } = new RabbitMQRetryPolicyOptions();
    }
}
