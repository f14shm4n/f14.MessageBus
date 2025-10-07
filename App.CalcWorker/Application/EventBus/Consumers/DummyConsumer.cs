using f14.MessageBus;

namespace App.CalcWorker.Application.EventBus.Consumers
{
    internal sealed class DummyConsumer : IConsumer<DummyConsumerMessage>
    {
        private readonly ILogger<DummyConsumer> _logger;

        public DummyConsumer(ILogger<DummyConsumer> logger)
        {
            _logger = logger;
        }

        public Task ConsumeAsync(DummyConsumerMessage message, CancellationToken token = default)
        {
            _logger.LogInformation($"[Id:{message.Id}] Description: '{message.Description}'");
            return Task.CompletedTask;
        }
    }
}
