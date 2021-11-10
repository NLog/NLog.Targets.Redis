using System;
using StackExchange.Redis;

namespace NLog.Targets.Redis
{
    internal class RedisConnectionManager : IDisposable
    {
        private IConnectionMultiplexer _connectionMultiplexer;
        private readonly int _db;

        public ConfigurationOptions ConfigurationOptions { get; }

        public RedisConnectionManager(string host, int port, int db, string password, string clientName = null, string configurationOptions = null)
        {
            _db = db;

            var connectionOptions = string.IsNullOrEmpty(configurationOptions) ? new ConfigurationOptions() : ConfigurationOptions.Parse(configurationOptions);
            connectionOptions.AbortOnConnectFail = false;
            connectionOptions.KeepAlive = 5;
            connectionOptions.EndPoints.Add(host, port);

            if (!string.IsNullOrEmpty(password))
            {
                connectionOptions.Password = password;
            }
            if (!string.IsNullOrEmpty(clientName))
            {
                connectionOptions.ClientName = clientName;
            }

            ConfigurationOptions = connectionOptions;
        }

        public void InitializeConnection()
        {
            _connectionMultiplexer = CreateConnectionMultiplexer();
        }

        protected virtual IConnectionMultiplexer CreateConnectionMultiplexer()
        {
            return ConnectionMultiplexer.Connect(ConfigurationOptions);
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

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _connectionMultiplexer?.Dispose();
            }
        }
    }
}