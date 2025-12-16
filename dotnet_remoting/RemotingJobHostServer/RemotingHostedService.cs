using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Runtime.Remoting;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Remoting.Channels.Http;
using Microsoft.Extensions.Logging;

namespace RemotingJobServer;

public class RemotingHostedService : IHostedService
{
    private readonly LegacyRemotingConfiguration _legacyRemotingConfiguration;

    private readonly ILogger<RemotingHostedService> _logger;

    public RemotingHostedService(
        LegacyRemotingConfiguration legacyRemotingConfiguration,
        ILogger<RemotingHostedService> logger)
    {
        _legacyRemotingConfiguration = legacyRemotingConfiguration;
        _logger = logger;
    }

    public void InitializeRemoting(LegacyRemotingConfiguration config)
    {
        if (config.UseAppConfig)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string exeName = AppDomain.CurrentDomain.FriendlyName;
            string executablePath = Path.Combine(baseDir, exeName);
            RemotingConfiguration.Configure(executablePath + ".config", false);
            return;
        }
        // register a channel
        var jobHttpChannel = new HttpChannel(4000);
        ChannelServices.RegisterChannel(jobHttpChannel, false);

        // allow full deserialization
        BinaryServerFormatterSinkProvider provider = null;
        provider = new BinaryServerFormatterSinkProvider
        {
            TypeFilterLevel = TypeFilterLevel.Full
        };

        var props = new Hashtable
        {
            ["port"] = 4001
        };
        var jobTcpChannel = new TcpChannel(props, null, provider);
        ChannelServices.RegisterChannel(jobTcpChannel, false);

        // register the object
        RemotingConfiguration.ApplicationName = "RemotingJobServer";
        RemotingConfiguration.RegisterWellKnownServiceType(
            typeof(JobServerInternal),
            "JobURI",
            WellKnownObjectMode.Singleton
        );
        RemotingConfiguration.RegisterWellKnownServiceType(
            typeof(JobNotesFactory),
            "JobNotes/Factory",
            WellKnownObjectMode.Singleton
        );
        RemotingConfiguration.RegisterActivatedServiceType(
            typeof(JobNotesInternal));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        InitializeRemoting(_legacyRemotingConfiguration);
        _logger.LogInformation("Remoting Server started");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
