using Shared.Commons.Options.Polly;

namespace Shared.RabbitMQ
{
    public class RabbitMQOptions
    {
        public string ConnectionString { get; set; } = string.Empty;
        public RetryPolicyOptions ConnectionRetryPolicy { get; set; } = new RetryPolicyOptions();
    }
}
