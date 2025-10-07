using f14.MessageBus;
using f14.MessageBus.Internals;
using FluentAssertions;
using Tests.SharedResources.EventBus.Consumers;
using Tests.SharedResources.EventBus.Messages;

namespace Tests.EventBus
{
    public class ConsumerManagerTests
    {
        #region TryAdd

        [Fact]
        public void TryAdd_Message_WithTwoConsumers()
        {
            var manager = CreateManager();

            manager.MessageTypesCount.Should().Be(0);
            manager.ConsumersCount.Should().Be(0);

            manager.TryAdd<Int32Message, Int32MessageConsumer>().Should().BeTrue();
            manager.TryAdd<Int32Message, AnotherInt32MessageConsumer>().Should().BeTrue();

            manager.MessageTypesCount.Should().Be(1);
            manager.ConsumersCount.Should().Be(2);

            manager.TryGetConsumers(typeof(Int32Message), out var consumers);
            consumers.Should().NotBeNull();
            consumers.Should().BeEquivalentTo([typeof(Int32MessageConsumer), typeof(AnotherInt32MessageConsumer)]);
        }

        [Fact]
        public void TryAdd_ManyMessages()
        {
            var manager = CreateManager();

            manager.MessageTypesCount.Should().Be(0);
            manager.ConsumersCount.Should().Be(0);

            manager.TryAdd<Int32Message, Int32MessageConsumer>().Should().BeTrue();
            manager.TryAdd<StringMessage, StringMessageConsumer>().Should().BeTrue();

            manager.MessageTypesCount.Should().Be(2);
            manager.ConsumersCount.Should().Be(2);

            manager.TryGetConsumers(typeof(Int32Message), out var consumers);
            consumers.Should().NotBeNull();
            consumers.Should().BeEquivalentTo([typeof(Int32MessageConsumer)]);

            manager.TryGetConsumers(typeof(StringMessage), out consumers);
            consumers.Should().NotBeNull();
            consumers.Should().BeEquivalentTo([typeof(StringMessageConsumer)]);
        }

        [Fact]
        public void TryAdd_SameMessage()
        {
            var manager = CreateManager();

            manager.MessageTypesCount.Should().Be(0);
            manager.ConsumersCount.Should().Be(0);

            manager.TryAdd<Int32Message, Int32MessageConsumer>().Should().BeTrue();
            manager.TryAdd<Int32Message, Int32MessageConsumer>().Should().BeFalse();
        }

        #endregion

        #region GetMessageTypeByName

        [Fact]
        public void GetMessageTypeByName_Empty()
        {
            var manager = CreateManager();

            manager.MessageTypesCount.Should().Be(0);
            manager.ConsumersCount.Should().Be(0);

            manager.GetMessageTypeByName(typeof(Int32Message).Name).Should().BeNull();
        }

        [Fact]
        public void GetMessageTypeByName_NotEmpty()
        {
            var manager = CreateManager();

            manager.MessageTypesCount.Should().Be(0);
            manager.ConsumersCount.Should().Be(0);

            manager.TryAdd<Int32Message, Int32MessageConsumer>().Should().BeTrue();
            manager.TryAdd<StringMessage, StringMessageConsumer>().Should().BeTrue();

            manager.GetMessageTypeByName(typeof(Int32Message).Name).Should().NotBeNull();
            manager.GetMessageTypeByName(typeof(StringMessage).Name).Should().NotBeNull();
        }

        #endregion

        #region Utils

        private static IConsumerManager CreateManager() => new ConsumerManager();

        #endregion
    }
}