# Original NLog.Targets.Redis API

```cs
[Target("Redis")]
public class RedisTarget : TargetWithLayout
{
    [Required]
    public string Key { get; set; }
    public string Host { get; set; } = "localhost"
    public int Port { get; set; } = "6379"
}
```

The assembly targeted net40 and NLog 2.0.0.2000.