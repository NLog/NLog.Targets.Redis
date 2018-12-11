using StackExchange.Redis;

namespace NLog.Targets.Redis.Tests.Mocks
{
    internal class MockRedisTarget : RedisTarget
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public MockRedisTarget()
        {
        }

        public MockRedisTarget(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }

        internal override RedisConnectionManager CreateConnectionManager()
        {
            return new MockRedisConnectionManager(Host, Port, Db, Password, _connectionMultiplexer);
        }
    }
}