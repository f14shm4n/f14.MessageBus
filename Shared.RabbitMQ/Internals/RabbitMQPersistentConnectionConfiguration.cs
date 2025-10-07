using f14.RetryPolly;

namespace Shared.RabbitMQ.Internals
{
    internal sealed class RabbitMQPersistentConnectionConfiguration : IRabbitMQPersistentConnectionConfiguration
    {
        private RetryPolicyInfo _retryPolicyOptions = new();

        public RetryPolicyInfo RetryPolicy
        {
            get => _retryPolicyOptions;
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                _retryPolicyOptions = value;
            }
        }
    }
}
