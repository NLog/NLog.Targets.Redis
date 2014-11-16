using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog.Config;
using NLog.Targets;
using NUnit.Framework;
using StackExchange.Redis;

namespace NLog.Redis.Tests
{
    /// <summary>
    /// These unit tests are marked ignore since they have a requirement of having a redis server running locally.
    /// </summary>
    [TestFixture, Ignore]
    public class RedisTargetTests
    {
        private const string RedisKey = "testkey";
        private const string RedisHost = "localhost";
        private const int RedisPort = 6379;

        
        [Test]
        public void Redis_Target_Should_Configure_With_List_DataType()
        {
            NLogRedisConfiguration("list");
        }

        [Test]
        public void Redis_Target_Should_Configure_With_Channel_DataType()
        {
            NLogRedisConfiguration("channel");
        }

        [Test]
        public void Redis_Target_Should_Put_Message_In_List_In_Redis()
        {
            NLogRedisConfiguration("list");

            var logger = LogManager.GetLogger("redis");
            logger.Info("test message");

            using (var redisConnection = GetRedisConnection())
            {
                var listValue = redisConnection.GetDatabase().ListLeftPop(RedisKey);
                Assert.IsFalse(!listValue.HasValue || listValue.IsNullOrEmpty);
                Assert.AreEqual("INFO test message", listValue.ToString());
            }
        }

        [Test]
        public void Redis_Target_Should_Put_Message_In_Channel_In_Redis()
        {
            _actionRun = false;
            NLogRedisConfiguration("channel");

            using (var redisConnection = GetRedisConnection())
            {
                var subscriber = redisConnection.GetSubscriber();
                subscriber.Subscribe(RedisKey, ListenForMessage);

                var logger = LogManager.GetLogger("redis");
                logger.Info("test pub/sub message");

                Thread.Sleep(1000);
                Assert.IsTrue(_actionRun);
            }
        }

        private bool _actionRun = false;
        public void ListenForMessage(RedisChannel channel, RedisValue value)
        {
            Assert.AreEqual(RedisKey, channel.ToString());
            Assert.IsFalse(!value.HasValue || value.IsNullOrEmpty);
            Assert.AreEqual("INFO test pub/sub message", value.ToString());
            _actionRun = true;
        }

        public void NLogRedisConfiguration(string dataType)
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

            // setup rules
            var rule1 = new LoggingRule("*", LogLevel.Info, redisTarget);
            config.LoggingRules.Add(rule1);

            LogManager.Configuration = config;
        }

        public ConnectionMultiplexer GetRedisConnection()
        {
            var connectionOptions = new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                SyncTimeout = 3000,
                ConnectTimeout = 3000,
                ConnectRetry = 3,
                KeepAlive = 5
            };
            connectionOptions.EndPoints.Add(RedisHost, RedisPort);

            return ConnectionMultiplexer.Connect(connectionOptions);
        }

    }
}
