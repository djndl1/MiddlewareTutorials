using System;
using Microsoft.Extensions.Logging;

namespace RemotingJobServer;

public static class LoggingFactory
{
    private static ILoggerFactory _factory;

    public static void Initialize(ILoggerFactory factory)
    {
        _factory = factory;
    }

    public static ILogger<T> GetLogger<T>() => _factory.CreateLogger<T>();
}
