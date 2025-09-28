using Shared.Commons.Options.Polly;

namespace Shared.RabbitMQ.Internals
{
    internal sealed class RabbitMQPersistentConnectionConfiguration : IRabbitMQPersistentConnectionConfiguration
    {
        private RetryPolicyOptions _retryPolicyOptions = new();

        public RetryPolicyOptions RetryPolicy
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
