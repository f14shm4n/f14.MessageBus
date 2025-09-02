using FluentAssertions;
using Shared.EventBus;
using Tests.SharedResources.Events;

namespace Tests.EventBus
{
    public class InMemoryRabbitMQSubscriptionsManagerTests
    {
        #region AddSubscription

        [Fact]
        public void AddSubscription_HaveCount_1()
        {
            var manager = CreateSubManager();
            manager.AddSubscription<Int32IntegrationEvent, Int32IntegrationEventHandler>();

            manager.GetSubscriptions(typeof(Int32IntegrationEvent))
                .Should()
                .HaveCount(1);
        }

        [Fact]
        public void AddSubscription_SingleEventType_HaveCount_2()
        {
            var manager = CreateSubManager();
            manager.AddSubscription<Int32IntegrationEvent, Int32IntegrationEventHandler>();
            manager.AddSubscription<Int32IntegrationEvent, Int32CustomizedIntegrationEventHandler>();

            manager.GetSubscriptions(typeof(Int32IntegrationEvent))
                .Should()
                .HaveCount(2);
        }

        [Fact]
        public void AddSubscription_VariouseEventType_HaveCount_3()
        {
            var manager = CreateSubManager();
            manager.AddSubscription<Int32IntegrationEvent, Int32IntegrationEventHandler>();
            manager.AddSubscription<Int32IntegrationEvent, Int32CustomizedIntegrationEventHandler>();
            manager.AddSubscription<StringIntegrationEvent, StringIntegrationEventHandler>();
            var col1 = manager.GetSubscriptions(typeof(Int32IntegrationEvent));
            var col2 = manager.GetSubscriptions(typeof(StringIntegrationEvent));

            (col1!.Count() + col2!.Count())
                .Should()
                .Be(3);
        }

        [Fact]
        public void AddSubscription_ShouldNotThrow()
        {
            var manager = CreateSubManager();
            FluentActions.Invoking(() =>
            {
                manager.AddSubscription<Int32IntegrationEvent, Int32IntegrationEventHandler>();
                manager.AddSubscription<StringIntegrationEvent, StringIntegrationEventHandler>();
            })
                .Should()
                .NotThrow();
        }

        [Fact]
        public void AddSubscription_ShouldThrowInvalidOperationException()
        {
            var manager = CreateSubManager();
            FluentActions.Invoking(() =>
            {
                manager.AddSubscription<Int32IntegrationEvent, Int32IntegrationEventHandler>();
                manager.AddSubscription<Int32IntegrationEvent, Int32IntegrationEventHandler>();
            })
                .Should()
                .Throw<InvalidOperationException>();
        }

        #endregion

        #region RemoveSubscription

        [Fact]
        public void RemoveSubscription_With_1_Handler()
        {
            var manager = CreateSubManager();
            manager.AddSubscription<Int32IntegrationEvent, Int32IntegrationEventHandler>();

            manager.GetSubscriptions(typeof(Int32IntegrationEvent))
                .Should()
                .NotBeEmpty();

            using (var eventMonitor = manager.Monitor())
            {
                manager.RemoveSubscription<Int32IntegrationEvent, Int32IntegrationEventHandler>();

                manager.GetSubscriptions(typeof(Int32IntegrationEvent))
                    .Should()
                    .BeNull();

                eventMonitor
                    .Should()
                    .Raise(nameof(InMemoryEventBusSubscriptionsManager.OnEventRemoved));
            }
        }


        [Fact]
        public void RemoveSubscription_SameEventType_With_2_Handler()
        {
            var manager = CreateSubManager();
            manager.AddSubscription<Int32IntegrationEvent, Int32IntegrationEventHandler>();
            manager.AddSubscription<Int32IntegrationEvent, Int32CustomizedIntegrationEventHandler>();

            manager.GetSubscriptions(typeof(Int32IntegrationEvent))
                .Should()
                .HaveCount(2);

            using (var eventMonitor = manager.Monitor())
            {
                manager.RemoveSubscription<Int32IntegrationEvent, Int32IntegrationEventHandler>();

                manager.GetSubscriptions(typeof(Int32IntegrationEvent))
                    .Should()
                    .HaveCount(1);

                eventMonitor
                    .Should()
                    .NotRaise(nameof(InMemoryEventBusSubscriptionsManager.OnEventRemoved));
            }
        }

        [Fact]
        public void RemoveSubscription_With_3_Handler()
        {
            var manager = CreateSubManager();
            manager.AddSubscription<Int32IntegrationEvent, Int32IntegrationEventHandler>();
            manager.AddSubscription<Int32IntegrationEvent, Int32CustomizedIntegrationEventHandler>();
            manager.AddSubscription<StringIntegrationEvent, StringIntegrationEventHandler>();

            manager.GetSubscriptions(typeof(Int32IntegrationEvent))
                .Should()
                .HaveCount(2);

            manager.GetSubscriptions(typeof(StringIntegrationEvent))
                .Should()
                .HaveCount(1);

            using (var eventMonitor = manager.Monitor())
            {
                manager.RemoveSubscription<StringIntegrationEvent, StringIntegrationEventHandler>();

                manager.GetSubscriptions(typeof(Int32IntegrationEvent))
                    .Should()
                    .HaveCount(2);

                manager.GetSubscriptions(typeof(StringIntegrationEvent))
                    .Should()
                    .BeNull();

                eventMonitor
                    .Should()
                    .Raise(nameof(InMemoryEventBusSubscriptionsManager.OnEventRemoved));
            }
        }

        #endregion

        #region GetEventTypeByName

        [Fact]
        public void GetEventTypeByName_Empty()
        {
            var manager = CreateSubManager();

            manager.GetEventTypeByName(typeof(Int32IntegrationEvent).Name)
                .Should()
                .BeNull();
        }

        [Fact]
        public void GetEventTypeByName()
        {
            var manager = CreateSubManager();
            manager.AddSubscription<Int32IntegrationEvent, Int32IntegrationEventHandler>();
            manager.AddSubscription<Int32IntegrationEvent, Int32CustomizedIntegrationEventHandler>();
            manager.AddSubscription<StringIntegrationEvent, StringIntegrationEventHandler>();

            manager.GetEventTypeByName(typeof(Int32IntegrationEvent).Name)
                .Should()
                .NotBeNull();
            manager.GetEventTypeByName(typeof(StringIntegrationEvent).Name)
                .Should()
                .NotBeNull();
        }

        #endregion

        #region Utils

        private static InMemoryEventBusSubscriptionsManager CreateSubManager() => new();

        #endregion
    }
}