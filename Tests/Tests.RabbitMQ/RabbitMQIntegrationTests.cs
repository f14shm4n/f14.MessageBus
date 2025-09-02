using FluentAssertions;

namespace Tests.RabbitMQ
{
    public class RabbitMQIntegrationTests : RabbitMQIntegrationTestBase
    {
        [Fact]
        [Trait("DockerPlatform", "Linux")]
        public async Task RabbitMQPersistentConnection_IsConnected()
        {
            await using (var con = CreateConnection())
            {
                (await con.TryConnectAsync()).Should().BeTrue();
                con.IsConnected.Should().BeTrue();
            }
        }

        [Fact]
        [Trait("DockerPlatform", "Linux")]
        public async Task RabbitMQPersistentPublishChannel_IsOpen()
        {
            await using (var con = CreateConnection())
            {
                await con.TryConnectAsync();
                await using (var channel = CreatePublishChannel(con))
                {
                    (await channel.TryOpenChannelAsync()).Should().BeTrue();
                    channel.IsOpen.Should().BeTrue();
                }
            }
        }

        [Fact]
        [Trait("DockerPlatform", "Linux")]
        public async Task RabbitMQPersistentConsumerChannel_IsOpen()
        {
            await using (var con = CreateConnection())
            {
                await con.TryConnectAsync();
                await using (var channel = CreateConsumerChannel(con))
                {
                    (await channel.TryOpenChannelAsync()).Should().BeTrue();
                    channel.IsOpen.Should().BeTrue();
                }
            }
        }
    }
}
