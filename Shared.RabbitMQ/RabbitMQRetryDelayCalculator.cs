namespace Shared.RabbitMQ
{
    public static class RabbitMQRetryDelayCalculator
    {
        public static TimeSpan CalculateDelay(this RabbitMQRetryPolicyOptions source, int retryAttempt)
        {
            return source.DelayStrategy switch
            {
                // Linear
                _ => TimeSpan.FromMilliseconds(source.RetryDelayInMilliseconds * retryAttempt),
            };
        }
    }
}
