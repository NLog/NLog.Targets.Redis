using System;
using StackExchange.Redis;
using System.Collections.Generic;

namespace NLog.Targets
{
    internal class RedisConnectionManager : IDisposable
    {
        private ConnectionMultiplexer _connectionMultiplexer;

        private readonly List<string> _hosts;
        private readonly int _db;
        private readonly string _password;

        public RedisConnectionManager(List<string> hosts, int db, string password)
        {
            _hosts = hosts;
            _db = db;
            _password = password;

            InitializeConnection();
        }

        [Obsolete("Use constructor with hosts instead")]
        public RedisConnectionManager(string host, int port, int db, string password)
        {
            _hosts = new List<string>();
            _hosts.Add($"{host}:{port}");
            _db = db;
            _password = password;

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
            foreach (var host in _hosts)
            {
                connectionOptions.EndPoints.Add(host);
            }

            if (!string.IsNullOrEmpty(_password))
            {
                connectionOptions.Password = _password;
            }

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
