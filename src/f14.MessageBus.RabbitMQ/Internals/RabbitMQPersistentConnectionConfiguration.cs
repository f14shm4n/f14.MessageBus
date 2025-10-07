using f14.MessageBus.RabbitMQ;
using f14.RetryPolly;

namespace f14.MessageBus.RabbitMQ.Internals
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
