using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.EventBus
{
    public interface ISubscriptionInfo
    {
        Type EventHandlerType { get; }
    }
}
