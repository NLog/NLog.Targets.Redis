using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLog.Targets.Redis
{
    public enum RedisChannelPattern
    {
        /// <summary>
        /// Will be treated as a pattern if it includes *.
        /// </summary>
        Auto = 0,
        /// <summary>
        /// Never a pattern.
        /// </summary>
        Literal = 1,
        /// <summary>
        /// Always a pattern.
        /// </summary>
        Pattern = 2
    }
}
