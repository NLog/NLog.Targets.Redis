using System.Threading;
using Xunit;

namespace NLog.Targets.Redis.Tests.IntegrationTests
{
    public class RedisTargetWithoutSecurityTests : RedisTargetTestsBase
    {
        [Fact(Skip = "Integration Test")]
        public void Redis_Target_Should_Configure_With_List_DataType()
        {
            NLogRedisConfiguration("list");
        }

        [Fact(Skip = "Integration Test")]
        public void Redis_Target_Should_Configure_With_Channel_DataType()
        {
            NLogRedisConfiguration("channel");
        }

        [Fact(Skip = "Integration Test")]
        public void Redis_Target_Should_Put_Message_In_List_In_Redis()
        {
            NLogRedisConfiguration("list");

            var logger = LogManager.GetLogger("redis");
            logger.Info("test message");

            using (var redisConnection = GetRedisConnection())
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
            NLogRedisConfiguration("channel");

            using (var redisConnection = GetRedisConnection())
            {
                var subscriber = redisConnection.GetSubscriber();
                subscriber.Subscribe(RedisKey, ListenForMessage);

                var logger = LogManager.GetLogger("redis");
                logger.Info("test pub/sub message");

                Thread.Sleep(1000);
                Assert.True(ActionRun);
            }
        }
    }

}
