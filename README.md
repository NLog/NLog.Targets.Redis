[![Build status](https://ci.appveyor.com/api/projects/status/mkkmh5695wtpvmu0/branch/master?svg=true)](https://ci.appveyor.com/project/RichClement/nlog-redis/branch/master)

NLog.Redis
==========

This project provides a custom target for the NLog framework to allow a user to send log messages to a Redis server. The target supports sending log messages to a Redis list or to a pub/sub channel. 

## Installation
NLog.Redis is available as a NuGet Package. Type the following command into the Nuget Package Manager Console window to install it:

    Install-Package NLog.Redis


## Usage

The `<target />` configuration section contains five required fields and one optional field.

- host (required): The host name or IP Address of the Redis server.
- port (required): The port the Redis server is listening on.
- key (required): The key that should be used to identify the Redis list or the pub/sub channel to which the log messages are being delivered.
- dataType (required): The Redis data type that should be used to store the log messages. This can be either `list` or `channel`.
- layout (required): The layout that defines the format of the message to be sent to the Redis target.
- db (optional): The Redis database id to store the log messages in, if the Redis database type `list` is chosen.


## Config File

    <nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" throwExceptions="true">
      <extensions>
        <add assembly="NLog.Redis" />
      </extensions>
      <targets>
        <target xsi:type="Redis" name="redis" host="127.0.0.1" port="3679" db="0" 
                key="logKey" dataType="list" 
                layout="${date:format=yyyyMMddHHmmss} ${uppercase:${level}} ${message}" />
      </targets>
      <rules>
        <logger name="*" minlevel="Info" writeTo="redis" />
      </rules>
    </nlog>
 
