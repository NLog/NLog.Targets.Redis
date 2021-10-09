using StackExchange.Redis;

namespace NLog.Targets.Redis.Tests.Mocks
{
    internal class MockRedisConnectionManager : RedisConnectionManager
    {
        private readonly IConnectionMultiplexer _multiplexer;

        public MockRedisConnectionManager(IConnectionMultiplexer multiplexer, string host, int port, int db, string password = null, string clientName = null, string configurationOptions = null)
            : base(host, port, db, password, clientName, configurationOptions)
        {
            _multiplexer = multiplexer;
        }

        public MockRedisConnectionManager(string host, int port, int db, string password = null, string clientName = null, string configurationOptions = null)
            : base(host, port, db, password, clientName, configurationOptions)
        {
        }

        protected override IConnectionMultiplexer CreateConnectionMultiplexer()
        {
            return _multiplexer;
        }
    }
}