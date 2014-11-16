using System;
using NLog.Config;

namespace NLog.Targets
{
    [Target("Redis")]
    public class RedisTarget : TargetWithLayout
    {
        protected const string ListDataType = "list";
        protected const string ChannelDataType = "channel";

        /// <summary>
        /// Sets the host name or IP Address of the redis server
        /// </summary>
        [RequiredParameter]
        public string Host { get; set; }

        /// <summary>
        /// Sets the port number redis is running on
        /// </summary>
        [RequiredParameter]
        public int Port { get; set; }

        /// <summary>
        /// Sets the key to be used for either the list or the pub/sub channel in redis
        /// </summary>
        [RequiredParameter]
        public string Key { get; set; }

        /// <summary>
        /// Sets what redis data type to use, either "list" or "channel"
        /// </summary>
        [RequiredParameter]
        public string DataType { get; set; }

        /// <summary>
        /// Sets the database id to be used in redis if the log entries are sent to a list. Defaults to 0
        /// </summary>
        public int Db { get; set; }

        protected RedisConnectionManager RedisConnectionManager;

        public RedisTarget()
        {
            Db = 0;
        }

        protected override void InitializeTarget()
        {
            base.InitializeTarget();

            RedisConnectionManager = new RedisConnectionManager(Host, Port, Db);
        }
        
        protected override void CloseTarget()
        {
            if (RedisConnectionManager != null)
            {
                RedisConnectionManager.Dispose();    
            }
            
            base.CloseTarget();
        }

        protected override void Write(LogEventInfo logEvent)
        {
            var message = this.Layout.Render(logEvent);
            var redisDatabase = RedisConnectionManager.GetDatabase();
            switch (DataType.ToLower())
            {
                case ListDataType:
                    redisDatabase.ListRightPush(Key, message);
                    break;
                case ChannelDataType:
                    redisDatabase.Publish(Key, message);
                    break;
                default:
                    throw new Exception("no data type defined for redis");
            }
        }

        protected override void Write(Common.AsyncLogEventInfo logEvent)
        {
            var message = this.Layout.Render(logEvent.LogEvent);
            var redisDatabase = RedisConnectionManager.GetDatabase();
            switch (DataType.ToLower())
            {
                case ListDataType:
                    redisDatabase.ListRightPushAsync(Key, message);
                    break;
                case ChannelDataType:
                    redisDatabase.PublishAsync(Key, message);
                    break;
                default:
                    throw new Exception("no data type defined for redis");
            }
        }

    }
}
