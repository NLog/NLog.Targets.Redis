using System;
using StackExchange.Redis;

namespace NLog.Targets
{
    public class RedisConnectionManager : IDisposable
    {
        private ConnectionMultiplexer _connectionMultiplexer;

        private readonly string _host;
        private readonly int _port;
        private readonly int _db;

        public RedisConnectionManager(string host, int port, int db)
        {
            _host = host;
            _port = port;
            _db = db;

            InitializeConnection();
        }

        private void InitializeConnection()
        {
            var connectionOptions = new ConfigurationOptions
                {
                    AbortOnConnectFail =  false,
                    SyncTimeout = 3000,
                    ConnectTimeout = 3000,
                    ConnectRetry = 3,
                    KeepAlive = 5
                };
            connectionOptions.EndPoints.Add(_host, _port);

            _connectionMultiplexer = ConnectionMultiplexer.Connect(connectionOptions);
        }

        public IDatabase GetDatabase()
        {
            if (_connectionMultiplexer == null) throw new Exception("connection manager not initialized");

            return _connectionMultiplexer.GetDatabase(_db);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_connectionMultiplexer != null)
                {
                    _connectionMultiplexer.Dispose();
                }
            }
        }
    }
}
