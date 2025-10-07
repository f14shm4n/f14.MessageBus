namespace f14.MessageBus.RabbitMQ
{
    /// <summary>
    /// Provides logic for troubleshooting errors that occur in the RabbitMQ worker process.
    /// </summary>
    public interface IRabbitMQErrorResolver
    {
        /// <summary>
        /// This method is triggered when the consumer is unable to process the message in the normal way.
        /// <para>
        /// Use this method to process the message in a specific way or send it to the dead-letter queue.
        /// </para>
        /// <para>
        /// <b>IMPORTANT:</b> this method must not throw any exceptions.
        /// </para>
        /// </summary>
        /// <param name="routingKey">Routing key.</param>
        /// <param name="body">Message body.</param>
        /// <param name="exception">Source exception.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Returns an action type indicating whether to acknowledge that the message was processed successfully or not.</returns>
        Task<ConsumerResolveAction> ResolveProcessingErrorAsync(string routingKey, ReadOnlyMemory<byte> body, Exception exception, CancellationToken cancellationToken = default);
    }
}
