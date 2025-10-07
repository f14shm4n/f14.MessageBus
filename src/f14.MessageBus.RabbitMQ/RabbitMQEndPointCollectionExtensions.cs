namespace f14.MessageBus.RabbitMQ
{
    public static class RabbitMQEndPointCollectionExtensions
    {
        public static Dictionary<string, HashSet<string>> MapRoutingKeyToExchange(this IRabbitMQEndPointCollection source)
        {
            ArgumentNullException.ThrowIfNull(source);

            Dictionary<string, HashSet<string>> map = [];
            foreach (var endpoint in source)
            {
                foreach (var key in endpoint.RoutingKeys)
                {
                    if (!map.TryGetValue(key, out var exhanges))
                    {
                        exhanges = [];
                        map[key] = exhanges;
                    }
                    exhanges.Add(endpoint.Exchange);
                }
            }
            return map;
        }
    }
}
