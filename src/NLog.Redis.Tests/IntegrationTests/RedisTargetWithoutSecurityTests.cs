using System.Threading;
using NUnit.Framework;

namespace NLog.Redis.Tests.IntegrationTests
{
    [TestFixture]
    [Ignore("Ignore")]
    public class RedisTargetWithoutSecurityTests : RedisTargetTestsBase
    {
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
            ActionRun = false;
            NLogRedisConfiguration("channel");

            using (var redisConnection = GetRedisConnection())
            {
                var subscriber = redisConnection.GetSubscriber();
                subscriber.Subscribe(RedisKey, ListenForMessage);

                var logger = LogManager.GetLogger("redis");
                logger.Info("test pub/sub message");

                Thread.Sleep(1000);
                Assert.IsTrue(ActionRun);
            }
        }
    }

}
