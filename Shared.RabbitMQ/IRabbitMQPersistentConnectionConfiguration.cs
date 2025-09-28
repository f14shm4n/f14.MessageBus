using Shared.Commons.Options.Polly;

namespace Shared.RabbitMQ
{
    public interface IRabbitMQPersistentConnectionConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentNullException">Throws an exception when attempting to set a null value.</exception>
        RetryPolicyOptions RetryPolicy { get; set; }
    }
}
