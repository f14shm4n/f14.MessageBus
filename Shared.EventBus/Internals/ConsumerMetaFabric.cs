using Microsoft.Extensions.DependencyInjection;

namespace Shared.EventBus.Internals
{
    internal class ConsumerMetaFabric : IConsumerMetaFabric
    {
        private readonly Dictionary<Type, ConsumerMetaInvoker> _invokers = [];

        public ConsumerMetaInvoker GetInvoker(IServiceScope scope, Type consumerType, Type messageType)
        {
            var consumer = ActivatorUtilities.CreateInstance(scope.ServiceProvider, consumerType);
            // If the invoker has not yet been created
            if (!_invokers.TryGetValue(consumerType, out ConsumerMetaInvoker? invoker))
            {
                invoker = new ConsumerMetaInvoker(typeof(IConsumer<>).MakeGenericType(messageType).GetMethod(Constants.ConsumeAsyncMethodName)!);
                _invokers[consumerType] = invoker;
            }
            // Do not forget to set the consumer instance to the invoker
            invoker.SetInstance(consumer);
            return invoker;
        }
    }
}
