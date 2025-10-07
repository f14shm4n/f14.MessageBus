using f14.MessageBus;
using f14.MessageBus.Internals;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tests.SharedResources.EventBus.Consumers;
using Tests.SharedResources.EventBus.Messages;

namespace Tests.EventBus
{
    public class MessageProcessorTests
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConsumerManager _consumerManager = new ConsumerManager();
        private readonly IMessageSerializer _messageSerializer = new JsonTextMessageSerializer();

        public MessageProcessorTests()
        {
            var scfMock = new Mock<IServiceScopeFactory>();
            var scMock = new Mock<IServiceScope>();
            var spMock = new Mock<IServiceProvider>();
            scfMock.Setup(x => x.CreateScope()).Returns(scMock.Object);
            scMock.Setup(x => x.ServiceProvider).Returns(spMock.Object);

            _scopeFactory = scfMock.Object;

            _consumerManager.TryAdd<Int32Message, Int32MessageConsumer>();
            _consumerManager.TryAdd<StringMessage, StringMessageConsumer>();
        }

        [Fact]
        public async Task ProcessMessageAsync_Should_NotFail()
        {
            var processor = GetMessageProcessor();
            var act = () => processor.ProcessMessageAsync(typeof(Int32Message).Name, _messageSerializer.Serialize(new Int32Message() { Value = 100 }));
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task ProcessMessageAsync_Should_Fail()
        {
            var processor = GetMessageProcessor();
            var act = () => processor.ProcessMessageAsync(typeof(Int64Message).Name, _messageSerializer.Serialize(new Int64Message() { Value = 100 }));
            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        private IMessageProcessor GetMessageProcessor()
        {
            return new MessageProcessor(_scopeFactory, _consumerManager, _messageSerializer, new ConsumerInvokerFabric());
        }
    }
}
