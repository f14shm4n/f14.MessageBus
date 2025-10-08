namespace f14.MessageBus
{
    internal static class Constants
    {
        /// <summary>
        /// Provides the name of the <see cref="IConsumer{TMessage}.ConsumeAsync(TMessage, CancellationToken)"/> method.
        /// </summary>
        public const string ConsumeAsyncMethodName = nameof(IConsumer<int>.ConsumeAsync);
    }
}
