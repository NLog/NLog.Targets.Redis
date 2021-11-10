using System;
using System.ComponentModel;
using NLog.Common;
using NLog.Config;
using NLog.Layouts;

namespace NLog.Targets.Redis
{
    [Target("Redis")]
    public class RedisTarget : TargetWithLayout
    {
        /// <summary>
        /// Sets the host name or IP Address of the redis server
        /// </summary>
        [RequiredParameter]
        public Layout Host { get; set; }

        /// <summary>
        /// Sets the port number redis is running on
        /// </summary>
        [RequiredParameter]
        public Layout Port { get; set; }

        /// <summary>
        /// Sets the key to be used for either the list or the pub/sub channel in redis
        /// </summary>
        [RequiredParameter]
        public Layout Key { get; set; }

        /// <summary>
        /// Sets what redis data type to use, either "list" or "channel", defaults to "list"
        /// </summary>
        [DefaultValue(RedisDataType.List)]
        public RedisDataType DataType { get; set; } = RedisDataType.List;

        /// <summary>
        /// Sets the database id to be used in redis if the log entries are sent to a list. Defaults to 0
        /// </summary>
        public Layout Db { get; set; }

        /// <summary>
        /// Sets the password to be used when accessing Redis with authentication required
        /// </summary>
        public Layout Password { get; set; }

        /// <summary>
        /// The client name to use for all connections
        /// </summary>
        public Layout ClientName { get; set; }

        /// <summary>
        /// Additional ConfigurationOptions in Comma-delimeted-Format. See also <see cref="StackExchange.Redis.ConfigurationOptions.Parse(string)"/>
        /// </summary>
        /// <remarks>
        /// See <a href="https://stackexchange.github.io/StackExchange.Redis/Configuration.html">Configuration Options</a>
        /// </remarks>
        public Layout ConfigurationOptions { get; set; }

        private RedisConnectionManager _redisConnectionManager;

        internal virtual RedisConnectionManager CreateConnectionManager(string host, int port, int db, string password, string clientName, string configurationOptions)
        {
            return new RedisConnectionManager(host, port, db, password, clientName, configurationOptions);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NLog.Targets.RedisTarget" /> class.
        /// </summary>
        public RedisTarget()
        {
            OptimizeBufferReuse = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NLog.Targets.RedisTarget" /> class.
        /// </summary>
        public RedisTarget(string name)
            :base()
        {
            Name = name;
        }

        protected override void InitializeTarget()
        {
            var host = Host?.Render(LogEventInfo.CreateNullEvent());
            var password = Password?.Render(LogEventInfo.CreateNullEvent());

            var renderedPort = Port.Render(LogEventInfo.CreateNullEvent());
            if (!int.TryParse(renderedPort, out var port))
            {
                throw new NLogConfigurationException($"Unable to parse Port:{renderedPort}");
            }

            var db = 0;
            if (Db != null)
            {
                var renderedDb = Db.Render(LogEventInfo.CreateNullEvent());
                if (!int.TryParse(renderedDb, out db))
                {
                    InternalLogger.Warn($"{nameof(RedisTarget)}: Unable to parse db as a number: {renderedDb} falling back to the default db 0");
                }
            }

            var clientName = ClientName?.Render(LogEventInfo.CreateNullEvent()) ?? string.Empty;
            var configurationOptions = ConfigurationOptions?.Render(LogEventInfo.CreateNullEvent()) ?? string.Empty;

            _redisConnectionManager = CreateConnectionManager(host, port, db, password, clientName, configurationOptions);
            _redisConnectionManager.InitializeConnection();

            base.InitializeTarget();
        }

        protected override void CloseTarget()
        {
            _redisConnectionManager?.Dispose();

            base.CloseTarget();
        }

        protected override void Write(LogEventInfo logEvent)
        {
            var message = RenderLogEvent(Layout, logEvent);
            
            var key = Key?.Render(logEvent);
            var redisDatabase = _redisConnectionManager.GetDatabase();
            switch (DataType)
            {
                case RedisDataType.List:
                    redisDatabase.ListRightPush(key, message);
                    break;
                case RedisDataType.Channel:
                    redisDatabase.Publish(key, message);
                    break;
                default:
                    throw new Exception($"The required {nameof(DataType)} property was not defined or is invalid. Consider specifying either {nameof(RedisDataType.List)} or {nameof(RedisDataType.Channel)}");
            }
        }
    }
}