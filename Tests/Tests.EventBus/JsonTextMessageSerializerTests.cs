using f14.MessageBus.Internals;
using FluentAssertions;
using System.Threading.Tasks;
using Tests.SharedResources.EventBus.Messages;

namespace Tests.EventBus
{
    public class JsonTextMessageSerializerTests
    {
        [Fact]
        public async Task Serialize_Deserialize_StringMessage()
        {
            var serializer = new JsonTextMessageSerializer();
            var message = new StringMessage { Value = "test" };
            var bytes = await serializer.SerializeAsync(message);
            var r = await serializer.DeserializeAsync(bytes, typeof(StringMessage)) as StringMessage;
            r.Should().NotBeNull();
            r.Value.Should().Be(message.Value);
        }

        [Fact]
        public async Task Serialize_Deserialize_Int32()
        {
            var serializer = new JsonTextMessageSerializer();
            var message = 5;
            var bytes = await serializer.SerializeAsync(message);
            var r = await serializer.DeserializeAsync(bytes, typeof(int));
            r.Should().NotBeNull();
            ((int)r).Should().Be(message);
        }
    }
}
