namespace Shared.RabbitMQ
{
    public class RabbitMQOptions
    {
        public string ConnectionString { get; set; } = string.Empty;
        public RabbitMQRetryPolicyOptions ConnectionRetryPolicy { get; set; } = new RabbitMQRetryPolicyOptions();
    }
}
