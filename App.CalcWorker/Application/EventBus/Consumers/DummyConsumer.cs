using Shared.EventBus;

namespace App.CalcWorker.Application.EventBus.Consumers
{
    internal readonly record struct DummyConsumerMessage
    {
        public int Id { get; init; }
        public string Description { get; init; }
    }

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
