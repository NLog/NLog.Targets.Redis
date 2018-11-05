using System.Threading;
using Xunit;

namespace NLog.Targets.Redis.Tests.IntegrationTests
{
    public class RedisTargetSecurityTests : RedisTargetTestsBase
    {
        [Fact(Skip = "Integration Test")]
        public void Redis_Target_Should_Configure_With_List_DataType()
        {
            NLogRedisConfiguration("list", true);
        }

        [Fact(Skip = "Integration Test")]
        public void Redis_Target_Should_Configure_With_Channel_DataType()
        {
            NLogRedisConfiguration("channel", true);
        }

        [Fact(Skip = "Integration Test")]
        public void Redis_Target_Should_Put_Message_In_List_In_Redis()
        {
            NLogRedisConfiguration("list", true);

            var logger = LogManager.GetLogger("redis");
            logger.Info("test message");

            using (var redisConnection = GetRedisConnection(true))
            {
                var listValue = redisConnection.GetDatabase().ListLeftPop(RedisKey);
                Assert.False(!listValue.HasValue || listValue.IsNullOrEmpty);
                Assert.Equal("INFO test message", listValue.ToString());
            }
        }

        [Fact(Skip = "Integration Test")]
        public void Redis_Target_Should_Put_Message_In_Channel_In_Redis()
        {
            ActionRun = false;
            NLogRedisConfiguration("channel", true);

            using (var redisConnection = GetRedisConnection(true))
            {
                var subscriber = redisConnection.GetSubscriber();
                subscriber.Subscribe(RedisKey, ListenForMessage);

                var logger = LogManager.GetLogger("redis");
                logger.Info("test pub/sub message");

                Thread.Sleep(1000);
                Assert.True(ActionRun);
            }
        }

        [Fact(Skip = "Integration Test")]
        public void Redis_Target_Should_Fail_To_Configure_WithList_DataType_Without_Password()
        {
            NLogRedisConfiguration("list");

            var logger = LogManager.GetLogger("redis");
            logger.Info("test message");

            using (var redisConnection = GetRedisConnection(true))
            {
                var listValue = redisConnection.GetDatabase().ListLeftPop(RedisKey);
                Assert.False(listValue.HasValue);
            }
        }

        [Fact(Skip = "Integration Test")]
        public void Redis_Target_Should_Fail_To_Configure_WithChannel_DataType_Without_Password()
        {
            ActionRun = false;
            NLogRedisConfiguration("channel");

            using (var redisConnection = GetRedisConnection(true))
            {
                var subscriber = redisConnection.GetSubscriber();
                subscriber.Subscribe(RedisKey, ListenForMessage);

                var logger = LogManager.GetLogger("redis");
                logger.Info("test pub/sub message");

                Thread.Sleep(1000);
                Assert.False(ActionRun);
            }
        }
    }
}
