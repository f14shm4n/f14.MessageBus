using f14.MessageBus.Internals;
using System.Reflection;

namespace f14.MessageBus
{
    public sealed class ConsumerMetaInvoker : IDisposable
    {
        private readonly MethodInfo _method;
        private object? _instance;

        public ConsumerMetaInvoker(MethodInfo method)
        {
            _method = method;
        }

        public void SetInstance(object instance)
        {
            _instance = instance;
        }

        public void Clear()
        {
            if (_instance != null)
            {
                _instance = null;
            }
        }

        public Task InvokeAsync(params object?[]? args)
        {
            if (_instance == null)
            {
                ThrowHelper.CantInvokeConsumerInstanceIsNull();
            }

            if (_method.Invoke(_instance, args) is Task task)
            {
                return task;
            }
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Clear();
        }
    }
}
