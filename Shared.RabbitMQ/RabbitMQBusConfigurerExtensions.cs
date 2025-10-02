namespace Shared.RabbitMQ
{
    public static class RabbitMQBusConfigurerExtensions
    {
        /// <summary>
        /// Configures the publisher endpoint with default exchange and queue setup.
        /// </summary>
        /// <typeparam name="TMessage">The endpoint message type to bind.</typeparam>
        /// <param name="source">Current bus configurer.</param>
        /// <param name="exchange">Exchange name.</param>
        /// <param name="queue">Queue name.</param>
        /// <returns>The current configurer.</returns>
        public static RabbitMQBusConfigurer PublishEndPoint<TMessage>(this RabbitMQBusConfigurer source, string exchange, string queue)
        {
            ArgumentNullException.ThrowIfNull(source);
            return source.PublishEndPoint(exchange, queue, endpoint => endpoint.Message<TMessage>());
        }

        /// <summary>
        /// Configures the consumer endpoint with default exchange and queue setup.
        /// </summary>
        /// <typeparam name="TMessage">The endpoint message type to bind</typeparam>
        /// <param name="source">Current bus configurer.</param>
        /// <param name="exchange">Exchange name.</param>
        /// <param name="queue">Queue name.</param>
        /// <returns>The current configurer.</returns>
        public static RabbitMQBusConfigurer ConsumeEndPoint<TMessage>(this RabbitMQBusConfigurer source, string exchange, string queue)
        {
            ArgumentNullException.ThrowIfNull(source);
            return source.ConsumeEndPoint(exchange, queue, endpoint => endpoint.Message<TMessage>());
        }

        /// <summary>
        /// Configures the publisher endpoint with default exchange and queue setup.
        /// <para>
        /// Exchange settings: <b>ExchangeType</b>: <i>'fanout'</i>, <b>durable</b>: <i>true</i>.
        /// </para>
        /// <para>
        /// Queue settings: <b>durable</b>: <i>true</i>, <b>exclusive</b>: <i>false</i>, <b>autoDelete</b>: <i>false</i>.
        /// </para>
        /// </summary>
        /// <param name="source">Current bus configurer.</param>
        /// <param name="exchange">Exchange name.</param>
        /// <param name="queue">Queue name.</param>
        /// <param name="configure">Endpoint configurator.</param>
        /// <returns>The current configurer.</returns>
        public static RabbitMQBusConfigurer PublishEndPoint(this RabbitMQBusConfigurer source, string exchange, string queue, Action<IRabbitMQEndPoint> configure)
        {
            ArgumentNullException.ThrowIfNull(source);
            source.PublishEndPoint(x => ConfigureDefaultEndPoint(x, exchange, queue, configure));
            return source;
        }

        /// <summary>
        /// Configures the consumer endpoint with default exchange and queue setup.
        /// <para>
        /// Exchange settings: <b>ExchangeType</b>: <i>'fanout'</i>, <b>durable</b>: <i>true</i>.
        /// </para>
        /// <para>
        /// Queue settings: <b>durable</b>: <i>true</i>, <b>exclusive</b>: <i>false</i>, <b>autoDelete</b>: <i>false</i>.
        /// </para>
        /// </summary>
        /// <param name="source">Current bus configurer.</param>
        /// <param name="exchange">Exchange name.</param>
        /// <param name="queue">Queue name.</param>
        /// <param name="configure">Endpoint configurator.</param>
        /// <returns>The current configurer.</returns>
        public static RabbitMQBusConfigurer ConsumeEndPoint(this RabbitMQBusConfigurer source, string exchange, string queue, Action<IRabbitMQEndPoint> configure)
        {
            ArgumentNullException.ThrowIfNull(source);
            source.ConsumeEndPoint(x => ConfigureDefaultEndPoint(x, exchange, queue, configure));
            return source;
        }

        // TODO: May this is not needed.
        //public static RabbitMQBusConfigurer ReplaceAsyncBasicConsumerFactory<TFactoryImpl>(this RabbitMQBusConfigurer source) where TFactoryImpl : class, IAsyncBasicConsumerFactory
        //{
        //    ArgumentNullException.ThrowIfNull(source);

        //    source.ReplaceService(ServiceDescriptor.Singleton<IAsyncBasicConsumerFactory, TFactoryImpl>());
        //    return source;
        //}

        private static void ConfigureDefaultEndPoint(IRabbitMQExchangeConfigurer configurer, string exchange, string queue, Action<IRabbitMQEndPoint> configure)
        {
            configurer
                .Exchange(exchange, ExchangeType.Fanout, durable: true)
                .Queue(queue, durable: true, exclusive: false, autoDelete: false)
                .EndPoint(configure);
        }
    }
}
