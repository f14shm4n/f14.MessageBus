using System.Collections;

namespace Shared.RabbitMQ
{
    public sealed class ExchangeType : IEnumerable<string>
    {
        #region Instance

        private readonly string _value;

        private ExchangeType(string type) => _value = type;

        public IEnumerator<string> GetEnumerator()
        {
            yield return Fanout._value;
            yield return Direct._value;
            yield return Topic._value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Static

        public readonly static ExchangeType Fanout = new("fanout");
        public readonly static ExchangeType Direct = new("direct");
        public readonly static ExchangeType Topic = new("topic");

        public static implicit operator string(ExchangeType type)
        {
            return type._value;
        }

        #endregion
    }
}
