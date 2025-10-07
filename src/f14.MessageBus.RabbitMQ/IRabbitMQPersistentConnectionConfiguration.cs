using f14.RetryPolly;

namespace f14.MessageBus.RabbitMQ
{
    public interface IRabbitMQPersistentConnectionConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentNullException">Throws an exception when attempting to set a null value.</exception>
        RetryPolicyInfo RetryPolicy { get; set; }
    }
}
