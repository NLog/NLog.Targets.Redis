using System;
using NLog.Targets.Redis.Tests.Mocks;
using NSubstitute;
using StackExchange.Redis;
using Xunit;

namespace NLog.Targets.Redis.Tests
{
    public class RedisConnectionManagerTests
    {
        [Fact]
        public void InitializeConnection_host_password_combo_should_connect()
        {
            var host = "abcde";
            var port = 1234;
            var db = 0;

            var connectionManager = new MockRedisConnectionManager(host, port, db);

            //Act
            connectionManager.InitializeConnection();

            //Assert
            Assert.Single(connectionManager.ConfigurationOptions.EndPoints);

            var endpoint = connectionManager.ConfigurationOptions.EndPoints[0];

            Assert.NotNull(endpoint);
            Assert.Contains(host + ':' + port, endpoint.ToString());
        }

        [Fact]
        public void InitializeConnection_password_should_be_set_if_provided()
        {
            var host = "abcde";
            var port = 1234;
            var db = 0;
            var password = "ABCDEFG";

            var connectionManager = new MockRedisConnectionManager(host, port, db, password: password);

            //Act
            connectionManager.InitializeConnection();

            //Assert
            var assertPassword = connectionManager.ConfigurationOptions.Password;
            Assert.NotNull(assertPassword);
            Assert.Equal(password, assertPassword);
        }

        [Fact]
        public void InitializeConnection_should_not_set_password_if_empty()
        {
            var host = "abcde";
            var port = 1234;
            var db = 0;
            var password = "";

            var connectionManager = new MockRedisConnectionManager(host, port, db, password: password);

            //Act
            connectionManager.InitializeConnection();

            //Assert
            var assertPassword = connectionManager.ConfigurationOptions.Password;
            Assert.Null(assertPassword);
        }
    }
}