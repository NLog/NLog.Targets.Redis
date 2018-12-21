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

        internal override RedisConnectionManager CreateConnectionManager(string host, int port, int db, string password)
        {
            return new MockRedisConnectionManager(host, port, db, password, _connectionMultiplexer);
        }
    }
}