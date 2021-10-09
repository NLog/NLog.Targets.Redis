using StackExchange.Redis;

namespace NLog.Targets.Redis.Tests.Mocks
{
    internal class MockRedisTarget : RedisTarget
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public ConfigurationOptions RedisConfiguration { get; private set; }

        public MockRedisTarget()
        {
        }

        public MockRedisTarget(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }

        internal override RedisConnectionManager CreateConnectionManager(string host, int port, int db, string password, string clientName, string configurationOptions)
        {
            var connectionManager = new MockRedisConnectionManager(_connectionMultiplexer, host, port, db, password, clientName, configurationOptions);
            RedisConfiguration = connectionManager.ConfigurationOptions;
            return connectionManager;
        }
    }
}