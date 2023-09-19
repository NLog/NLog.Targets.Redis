[![Build status](https://ci.appveyor.com/api/projects/status/ounar0m1gdidqram/branch/master?svg=true)](https://ci.appveyor.com/project/nlog/nlog-redis/branch/master)
[![NuGet](https://img.shields.io/nuget/vpre/NLog.Targets.Redis.svg)](https://www.nuget.org/packages/NLog.Targets.Redis)


NLog.Targets.Redis
==========

This project provides a custom target for the [NLog](https://github.com/NLog/NLog/) framework to allow a user to send log messages to a Redis server. The target supports sending log messages to a Redis list or to a pub/sub channel. 

## Installation
NLog.Targets.Redis is available as a NuGet Package. Type the following command into the Nuget Package Manager Console window to install it:

    Install-Package NLog.Targets.Redis


## Usage

The `<target />` configuration section contains five required fields and one optional field.

- host (required): The host name or IP Address of the Redis server.
- port (required): The port the Redis server is listening on, defaults to `6379`.
- key (required): The key that should be used to identify the Redis list or the pub/sub channel to which the log messages are being delivered.
- dataType (optional): The Redis data type that should be used to store the log messages. This can be either `list` or `channel`, defaults to `list`.
- channelPattern (optional): The Redis Channel Pattern to use when dataType = `channel`. This can be either `literal` or `pattern`, defaults to `auto`. 
- layout (required): The layout that defines the format of the message to be sent to the Redis target.
- db (optional): The Redis database id to store the log messages in, if the Redis database type `list` is chosen.
- clientName (optional): The Client name to use for all Redis connections.
- configurationOptions (optional): Additional configuration options (comma delimited). See [Configuration Options](https://stackexchange.github.io/StackExchange.Redis/Configuration.html)

## Config File

```xml
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" throwExceptions="true">
  <extensions>
    <add assembly="NLog.Targets.Redis" />
  </extensions>
  <targets>
    <target xsi:type="Redis" name="redis" host="127.0.0.1" port="6379" db="0" 
            key="logKey" dataType="list" 
            layout="${longdate} ${level:uppercase=true} ${message} ${exception:format=tostring}" />
  </targets>
  <rules>
    <logger name="*" minlevel="Info" writeTo="redis" />
  </rules>
</nlog>
```
 
## Notes

This is a fork of https://github.com/richclement/NLog.Redis


## Admin

### How to release
1. Create a new release with a new tag
2. Wait for the build 
