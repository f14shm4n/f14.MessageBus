using RabbitMQ.Client;
using System.Collections;

namespace Shared.RabbitMQ.Internals
{
    internal sealed class RabbitMQDeclarator : IRabbitMQDeclarator, IRabbitMQDeclarationCollection
    {
        private readonly Dictionary<string, Func<IChannel, CancellationToken, Task>> _exchangeDeclarations = [];
        private readonly Dictionary<string, Func<IChannel, CancellationToken, Task>> _queueDeclarations = [];

        public IReadOnlyCollection<Func<IChannel, CancellationToken, Task>> ExchangeDeclarations => _exchangeDeclarations.Values;
        public IReadOnlyCollection<Func<IChannel, CancellationToken, Task>> QueueDeclarations => _queueDeclarations.Values;

        public int Count => ExchangeDeclarations.Count + QueueDeclarations.Count;

        public void ExchangeDeclare(string exchange, string type, bool durable = false, bool autoDelete = false, IDictionary<string, object?>? arguments = null, bool passive = false, bool noWait = false)
        {
            _exchangeDeclarations[exchange] = (c, ct) => c.ExchangeDeclareAsync(exchange, type, durable, autoDelete, arguments, passive, noWait, ct);
        }

        public void QueueDeclare(string queue, bool durable = false, bool exclusive = true, bool autoDelete = true, IDictionary<string, object?>? arguments = null, bool noWait = false)
        {
            _queueDeclarations[queue] = (c, ct) => c.QueueDeclareAsync(queue, durable, exclusive, autoDelete, arguments, noWait, ct);
        }

        public void DefaultExchangeDeclare(string exchange)
        {
            ExchangeDeclare(exchange, RabbitMQConstants.ExchangeTypes.Fanout, durable: true);
        }

        public void DefaultQueueDeclare(string queue)
        {
            QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false);
        }

        public IEnumerator<Func<IChannel, CancellationToken, Task>> GetEnumerator()
        {
            return ExchangeDeclarations.Concat(QueueDeclarations).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
