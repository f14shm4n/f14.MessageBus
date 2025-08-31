namespace Shared.RabbitMQ
{
    public class RabbitMQRetryPolicyOptions
    {
        public int RetryCount { get; set; } = 3;
        public int RetryDelayInMilliseconds { get; set; } = 3000;
        public RetryDelayStrategy DelayStrategy { get; set; } = RetryDelayStrategy.Linear;
    }
}
