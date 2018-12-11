using NLog.Config;
using StackExchange.Redis;
using Xunit;

namespace NLog.Targets.Redis.Tests.IntegrationTests
{
    /// <summary>
    /// These unit tests are marked ignore since they have a requirement of having a redis server running locally.
    /// </summary>
    public abstract class RedisTargetTestsBase
    {
        protected const string RedisKey = "testkey";
        protected const string RedisHost = "localhost";
        protected const int RedisPort = 6379;
        protected const string RedisPassword = "testingpassword";

        protected bool ActionRun;
        public void ListenForMessage(RedisChannel channel, RedisValue value)
        {
            Assert.Equal(RedisKey, channel.ToString());
            Assert.False(!value.HasValue || value.IsNullOrEmpty);
            Assert.Equal("INFO test pub/sub message", value.ToString());
            ActionRun = true;
        }

        public void NLogRedisConfiguration(string dataType, bool usePassword = false)
        {
            // create config
            var config = new LoggingConfiguration();

            // create target
            var redisTarget = new RedisTarget();
            config.AddTarget("redis", redisTarget);

            // set target properties
            redisTarget.Layout = "${uppercase:${level}} ${message}";
            redisTarget.Host = RedisHost;
            redisTarget.Port = RedisPort;
            redisTarget.Key = RedisKey;
            redisTarget.Db = 0;
            redisTarget.DataType = dataType;
            if (usePassword) redisTarget.Password = RedisPassword;

            // setup rules
            var rule1 = new LoggingRule("*", LogLevel.Info, redisTarget);
            config.LoggingRules.Add(rule1);

            LogManager.Configuration = config;
        }

        public ConnectionMultiplexer GetRedisConnection(bool usePassword = false)
        {
            var connectionOptions = new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                SyncTimeout = 3000,
                ConnectTimeout = 3000,
                ConnectRetry = 3,
                KeepAlive = 5
            };
            if (usePassword) connectionOptions.Password = RedisPassword;

            connectionOptions.EndPoints.Add(RedisHost, RedisPort);

            return ConnectionMultiplexer.Connect(connectionOptions);
        }
    }
}
