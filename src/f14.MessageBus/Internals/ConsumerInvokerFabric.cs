using Microsoft.Extensions.DependencyInjection;

namespace f14.MessageBus.Internals
{
    internal class ConsumerInvokerFabric : IConsumerInvokerFabric
    {
        private readonly Dictionary<Type, ConsumerInvoker> _invokers = [];

        public ConsumerInvoker GetInvoker(IServiceScope scope, Type consumerType, Type messageType)
        {
            var consumer = ActivatorUtilities.CreateInstance(scope.ServiceProvider, consumerType);
            // If the invoker has not yet been created
            if (!_invokers.TryGetValue(consumerType, out ConsumerInvoker? invoker))
            {
                invoker = new ConsumerInvoker(typeof(IConsumer<>).MakeGenericType(messageType).GetMethod(Constants.ConsumeAsyncMethodName)!);
                _invokers[consumerType] = invoker;
            }
            // Do not forget to set the consumer instance to the invoker
            invoker.SetInstance(consumer);
            return invoker;
        }
    }
}
