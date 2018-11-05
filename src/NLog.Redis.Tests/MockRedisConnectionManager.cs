using NLog.Targets;
using StackExchange.Redis;

namespace NLog.Redis.Tests
{
    internal class MockRedisConnectionManager : RedisConnectionManager
    {
        private readonly IConnectionMultiplexer _multiplexer;
        public ConfigurationOptions ConfigurationOptions;

        public MockRedisConnectionManager(string host, int port, int db, string password,
            IConnectionMultiplexer multiplexer) : base(host, port, db, password)
        {
            _multiplexer = multiplexer;
        }

        public MockRedisConnectionManager(string host, int port, int db, string password)
            : base(host, port, db, password)
        {
        }

        protected override IConnectionMultiplexer CreateConnectionMultiplexer(ConfigurationOptions connectionOptions)
        {
            ConfigurationOptions = connectionOptions;
            return _multiplexer;
        }
    }
}