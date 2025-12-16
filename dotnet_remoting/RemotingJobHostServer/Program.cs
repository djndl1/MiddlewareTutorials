using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using RemotingContract;

namespace RemotingJobServer;

static class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            var builder = Host.CreateApplicationBuilder(args);

            LegacyRemotingConfiguration legacyRemotingConfig = builder.Configuration
                                                                    .GetSection("LegacyRemoting")
                                                                    .Get<LegacyRemotingConfiguration>();
            builder.Services.AddSingleton(legacyRemotingConfig);
            builder.Services.AddTransient<IJobServer, JobServerInternal>();
            builder.Services.AddHostedService<RemotingHostedService>();

            var host = builder.Build();
            var loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
            LoggingFactory.Initialize(loggerFactory);

            CompositionRoot.InitializeContainer(host.Services);

            host.Run();
        }
        catch (Exception e)
        {
            throw;
        }
    }
}
