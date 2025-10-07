using Microsoft.Extensions.DependencyInjection;

namespace f14.MessageBus
{
    public interface IConsumerMetaFabric
    {
        ConsumerMetaInvoker GetInvoker(IServiceScope scope, Type consumerType, Type messageType);
    }
}
