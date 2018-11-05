using System;
using NLog.Config;
using NLog.Layouts;

namespace NLog.Targets.Redis
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
        public string Key
        {
            get
            {
                SimpleLayout simpleLayout = _key as SimpleLayout;
                if (simpleLayout != null)
                    return simpleLayout.Text;
                else if (_key != null)
                    return _key.ToString();
                else
                    return null;
            }
            set { _key = value; }
        }

        private Layout _key;

        /// <summary>
        /// Sets what redis data type to use, either "list" or "channel"
        /// </summary>
        [RequiredParameter]
        public string DataType { get; set; }

        private string _dataTypeToLower;

        /// <summary>
        /// Sets the database id to be used in redis if the log entries are sent to a list. Defaults to 0
        /// </summary>
        public int Db { get; set; }

        /// <summary>
        /// Sets the password to be used when accessing Redis with authentication required
        /// </summary>
        public string Password { get; set; }

        private RedisConnectionManager _redisConnectionManager;

        internal virtual RedisConnectionManager CreateConnectionManager()
        {
            return new RedisConnectionManager(Host, Port, Db, Password);
        }

        protected override void InitializeTarget()
        {
            _dataTypeToLower = DataType?.ToLower();
            _redisConnectionManager = CreateConnectionManager();
            _redisConnectionManager.InitializeConnection();

            base.InitializeTarget();
        }

        protected override void CloseTarget()
        {
            if (_redisConnectionManager != null)
            {
                _redisConnectionManager.Dispose();
            }

            base.CloseTarget();
        }

        protected override void Write(LogEventInfo logEvent)
        {
            var message = this.Layout.Render(logEvent);
            var key = _key?.Render(logEvent);
            var redisDatabase = _redisConnectionManager.GetDatabase();
            switch (_dataTypeToLower)
            {
                case ListDataType:
                    redisDatabase.ListRightPush(key, message);
                    break;
                case ChannelDataType:
                    redisDatabase.Publish(key, message);
                    break;
                default:
                    throw new Exception("no data type defined for redis");
            }
        }
    }
}