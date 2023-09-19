using System;
using StackExchange.Redis;

namespace NLog.Targets.Redis
{
    internal class RedisConnectionManager : IDisposable
    {
        private IConnectionMultiplexer _connectionMultiplexer;
        private readonly int _db;

        public ConfigurationOptions ConfigurationOptions { get; }

        private Tuple<string, RedisChannel> _prevChannel;

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

        public void PushList(string key, string message)
        {
            GetDatabase().ListRightPush(key, message);
        }

        public void PushChannel(RedisChannelPattern pattern, string key, string message)
        {
            GetDatabase().Publish(ResolveChannel(pattern, key ?? string.Empty), message);
        }

        private RedisChannel ResolveChannel(RedisChannelPattern pattern, string key)
        {
            var redisChannel = _prevChannel;
            if (redisChannel is null || !string.Equals(redisChannel.Item1, key))
            {
                redisChannel = new Tuple<string, RedisChannel>(key, CreateChannel(pattern, key));
                _prevChannel = redisChannel;
            }
            return redisChannel.Item2;
        }

        private RedisChannel CreateChannel(RedisChannelPattern pattern, string key)
        {
            switch (pattern)
            {
                case RedisChannelPattern.Literal:
                    return new RedisChannel(key, RedisChannel.PatternMode.Literal);
                case RedisChannelPattern.Pattern:
                    return new RedisChannel(key, RedisChannel.PatternMode.Pattern);
                default:
                    return new RedisChannel(key, RedisChannel.PatternMode.Auto);
            }
        }

        private IDatabase GetDatabase()
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