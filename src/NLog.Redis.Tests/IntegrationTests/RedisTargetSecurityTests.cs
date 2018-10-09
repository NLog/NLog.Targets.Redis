using System.Threading;
using NUnit.Framework;

namespace NLog.Redis.Tests.IntegrationTests
{
    [TestFixture]
    [Ignore("Ignore")]
    public class RedisTargetSecurityTests : RedisTargetTestsBase
    {
        [SetUp]
        public void Setup()
        {
            Password = RedisPassword;
        }

        [TearDown]
        public void Teardown()
        {
            LogManager.Configuration = null;
        }

        [Test]
        public void Redis_Target_Should_Configure_With_List_DataType()
        {
            NLogRedisConfiguration("list", true);
        }

        [Test]
        public void Redis_Target_Should_Configure_With_Channel_DataType()
        {
            NLogRedisConfiguration("channel", true);
        }

        [Test]
        public void Redis_Target_Should_Put_Message_In_List_In_Redis()
        {
            NLogRedisConfiguration("list", true);

            var logger = LogManager.GetLogger("redis");
            logger.Info("test message");

            using (var redisConnection = GetRedisConnection(true))
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
            NLogRedisConfiguration("channel", true);

            using (var redisConnection = GetRedisConnection(true))
            {
                var subscriber = redisConnection.GetSubscriber();
                subscriber.Subscribe(RedisKey, ListenForMessage);

                var logger = LogManager.GetLogger("redis");
                logger.Info("test pub/sub message");

                Thread.Sleep(1000);
                Assert.IsTrue(ActionRun);
            }
        }

        [Test]
        public void Redis_Target_Should_Fail_To_Configure_WithList_DataType_Without_Password()
        {
            Password = null;
            NLogRedisConfiguration("list", false);

            var logger = LogManager.GetLogger("redis");
            logger.Info("test message");

            using (var redisConnection = GetRedisConnection(true))
            {
                var listValue = redisConnection.GetDatabase().ListLeftPop(RedisKey);
                Assert.IsFalse(listValue.HasValue);
            }
        }

        [Test]
        public void Redis_Target_Should_Fail_To_Configure_WithChannel_DataType_Without_Password()
        {
            ActionRun = false;
            NLogRedisConfiguration("channel", false);

            using (var redisConnection = GetRedisConnection(true))
            {
                var subscriber = redisConnection.GetSubscriber();
                subscriber.Subscribe(RedisKey, ListenForMessage);

                var logger = LogManager.GetLogger("redis");
                logger.Info("test pub/sub message");

                Thread.Sleep(1000);
                Assert.IsFalse(ActionRun);
            }
        }
    }
}
