namespace Shared.RabbitMQ.App
{
    public class RabbitMQExchangeOptions
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsDurable { get; set; } = true;
        public string Queue { get; set; } = string.Empty;
    }
}
