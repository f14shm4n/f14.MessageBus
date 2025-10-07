namespace App.CalcWorker.Application.EventBus.Consumers
{
    internal readonly record struct DummyConsumerMessage
    {
        public int Id { get; init; }
        public string Description { get; init; }
    }
}
