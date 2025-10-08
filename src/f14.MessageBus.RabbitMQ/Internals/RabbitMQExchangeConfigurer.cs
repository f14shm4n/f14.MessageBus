namespace f14.MessageBus.RabbitMQ.Internals
{
    internal sealed class RabbitMQExchangeConfigurer : IRabbitMQExchangeConfigurer, IRabbitMQQueueConfigurer, IRabbitMQEndPointConfigurer
    {
        private readonly RabbitMQDeclarator _declarator;
        private readonly RabbitMQEndPoints _endPoints;

        private string? _exchange;
        private string? _queue;

        public RabbitMQExchangeConfigurer(RabbitMQDeclarator declarator, RabbitMQEndPoints endPoints)
        {
            _declarator = declarator;
            _endPoints = endPoints;
        }

        internal bool IsCompleted { get; private set; }

        public IRabbitMQQueueConfigurer Exchange(string exchange, ExchangeType type, bool durable = false, bool autoDelete = false, IDictionary<string, object?>? arguments = null, bool passive = false, bool noWait = false)
        {
            _declarator.ExchangeDeclare(exchange, type, durable, autoDelete, arguments, passive, noWait);
            _exchange = exchange;
            return this;
        }

        public IRabbitMQEndPointConfigurer Queue(string queue, bool durable = false, bool exclusive = true, bool autoDelete = true, IDictionary<string, object?>? arguments = null, bool noWait = false)
        {
            _declarator.QueueDeclare(queue, durable, exclusive, autoDelete, arguments, noWait);
            _queue = queue;
            return this;
        }

        public void EndPoint(Action<IRabbitMQEndPoint> configure)
        {
            if (_exchange is null || _queue is null)
            {
                ThrowHelper.ExchangeOrQueueNamesIsNotSet();
            }
            configure(_endPoints.RegistedEndPoint(_exchange, _queue));
        }
    }
}
