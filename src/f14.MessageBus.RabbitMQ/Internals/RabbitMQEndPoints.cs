using System.Collections;

namespace f14.MessageBus.RabbitMQ.Internals
{
    internal sealed class RabbitMQEndPoints : IRabbitMQEndPoints, IRabbitMQEndPointCollection
    {
        private readonly Dictionary<string, RabbitMQEndPoint> _endpoints = [];

        public int Count => _endpoints.Count;

        public IRabbitMQEndPoint RegistedEndPoint(string exchange, string queue)
        {
            var key = string.Join('_', exchange, queue);
            var endpoint = new RabbitMQEndPoint(exchange, queue);
            _endpoints[key] = endpoint;
            return endpoint;
        }

        public IEnumerator<IRabbitMQEndPoint> GetEnumerator() => _endpoints.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
