using FluentAssertions;

namespace Tests.RabbitMQ
{
    public class RabbitMQIntegrationTests : RabbitMQIntegrationTestBase
    {
        [Fact]
        [Trait("DockerPlatform", "Linux")]
        public async Task RabbitMQPersistentConnection_IsConnected()
        {
            await using (var con = CreateConnection(CreateOptions()))
            {
                var r = await con.TryConnectAsync();

                r.Should().BeTrue();
                con.IsConnected.Should().BeTrue();
            }
        }
    }
}
