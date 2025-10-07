using Microsoft.Extensions.DependencyInjection;

namespace Shared.EventBus
{
    public interface IConsumerMetaFabric
    {
        ConsumerMetaInvoker GetInvoker(IServiceScope scope, Type consumerType, Type messageType);
    }
}
