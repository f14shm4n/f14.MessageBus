using f14.MessageBus.RabbitMQ;
using RabbitMQ.Client;

namespace f14.MessageBus.RabbitMQ.Internals
{
    internal sealed class RabbitMQEndPoint(string exchange, string queue) : IRabbitMQEndPoint
    {
        private readonly string _exchange = exchange;
        private readonly string _queue = queue;
        private readonly Dictionary<string, Func<IChannel, CancellationToken, Task>> _queueBindings = [];
        private readonly HashSet<string> _routingKeys = [];

        public string Exchange => _exchange;

        public string Queue => _queue;

        public IReadOnlyCollection<Func<IChannel, CancellationToken, Task>> Bindings => _queueBindings.Values;

        public IReadOnlySet<string> RoutingKeys => _routingKeys;

        public void Message<TMessage>()
        {
            BindQueue(typeof(TMessage).Name);
        }

        private void BindQueue(string routingKey, IDictionary<string, object?>? arguments = null, bool noWait = false)
        {
            ArgumentException.ThrowIfNullOrEmpty(routingKey);

            _routingKeys.Add(routingKey);
            _queueBindings[routingKey] = (c, ct) => c.QueueBindAsync(_queue, _exchange, routingKey, arguments, noWait, cancellationToken: ct);
        }
    }
}
