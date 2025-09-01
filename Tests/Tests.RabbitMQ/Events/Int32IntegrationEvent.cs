using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Tests.RabbitMQ.Events
{

    public record Int32IntegrationEvent : PrimitiveIntegrationEvent<int>
    {

    }
}
