using NLog.Config;
using NLog.Targets.Redis.Tests.Mocks;
using NSubstitute;
using StackExchange.Redis;
using Xunit;

namespace NLog.Targets.Redis.Tests
{
    public class RedisTargetTests
    {
        protected const string RedisKey = "testkey";
        protected const string RedisHost = "localhost";
        protected const string RedisPort = "6379";


        [Fact]
        public void RedisTarget_should_configure_with_list_DataType()
        {
            var mock = new MockRedisTarget();
            NLogRedisConfiguration(mock, RedisDataType.List);
        }

        [Fact]
        public void RedisTarget_should_default_to_list_DataType_prop()
        {
            var mock = new MockRedisTarget();
            NLogRedisConfiguration(mock);
            Assert.Equal(RedisDataType.List, mock.DataType);
        }

        [Fact]
        public void RedisTarget_should_configure_with_channel_DataType()
        {
            var mock = new MockRedisTarget();
            NLogRedisConfiguration(mock, RedisDataType.Channel);
        }

        [Fact]
        public void RedisTarget_should_put_message_in_list()
        {
            var multiplex = Substitute.For<IConnectionMultiplexer>();
            var database = Substitute.For<IDatabase>();
            multiplex.GetDatabase(Arg.Any<int>()).Returns(database);

            NLogRedisConfiguration(new MockRedisTarget(multiplex), RedisDataType.List);

            var logger = LogManager.GetLogger("redis");
            logger.Info("test message");

            database.Received().ListRightPush(RedisKey, "INFO test message");
        }

        [Fact]
        public void RedisTarget_should_put_message_in_channel()
        {
            var multiplex = Substitute.For<IConnectionMultiplexer>();
            var database = Substitute.For<IDatabase>();
            multiplex.GetDatabase(Arg.Any<int>()).Returns(database);

            NLogRedisConfiguration(new MockRedisTarget(multiplex), RedisDataType.Channel);

            var logger = LogManager.GetLogger("redis");
            logger.Info("test pub/sub message");

            database.Received().Publish(RedisKey, "INFO test pub/sub message");
        }

        [Fact]
        public void RedisTarget_should_default_to_list_if_no_DataType()
        {
            var multiplex = Substitute.For<IConnectionMultiplexer>();
            var database = Substitute.For<IDatabase>();
            multiplex.GetDatabase(Arg.Any<int>()).Returns(database);

            NLogRedisConfiguration(new MockRedisTarget(multiplex));

            var logger = LogManager.GetLogger("redis");
            logger.Info("test message");

            database.Received().ListRightPush(RedisKey, "INFO test message");
        }

        private void NLogRedisConfiguration(RedisTarget redisTarget, RedisDataType? dataType = null)
        {
            // create config
            var config = new LoggingConfiguration();

            // create target
            config.AddTarget("redis", redisTarget);

            // set target properties
            redisTarget.Layout = "${uppercase:${level}} ${message}";
            redisTarget.Host = RedisHost;
            redisTarget.Port = RedisPort;
            redisTarget.Key = RedisKey;
            redisTarget.Db = "0";
            if (dataType != null)
            {
                redisTarget.DataType = dataType.Value;
            }

            // setup rules
            var rule1 = new LoggingRule("*", LogLevel.Info, redisTarget);
            config.LoggingRules.Add(rule1);

            LogManager.Configuration = config;
        }
    }
}